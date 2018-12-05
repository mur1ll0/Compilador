using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AnalisadorLexico
{
    class Program
    {
        //Tabela de Símbolos
        public class ts
        {
            public String rotulo;
            public String estado;
            public int posicao;
            public Estado estadoGLC;
            public Symbol simbolo;
        }

        //Automato Finito Deterministico
        public class AFD
        {
            public List<string> afdCol;

            public AFD()
            {
                afdCol = new List<string>();
            }

        }


        //Simbolos: Todos os simbolos terminais e não terminais da linguagem
        public class Symbol
        {
            public Int32 Index;
            public string Name;
        }

        //Estados: Numero e qual symbolo o estado aceita
        public class Estado
        {
            public Int32 Index;
            public Symbol AcceptSymbol;
        }


        //Produções: Todas as producoes da linguagem
        public class Production
        {
            public Int32 Index;
            public Symbol NonTerminalIndex;
            public Int32 SymbolCount;
            public List<ProductionSymbol> ProductionSymbol;

            public Production()
            {
                ProductionSymbol = new List<ProductionSymbol>();
                SymbolCount = 0;
            }
        }

        //Tabela LALR: Reduções Saltos e Empilhamentos

        public class LALRState
        {
            public Int32 Index;
            public Int32 ActionCount;
            public List<LALRAction> LALRAction;
            public LALRState() {
                LALRAction = new List<LALRAction>();
            }
        }

        public class LALRAction
        {
            public Symbol SymbolIndex;
            public Int32 Action;
            public Int32 Value;
        }

        public class ProductionSymbol
        {
            public Symbol Symbol;
        }

        public class Mapeamento
        {
            public List<DadosMapeamento> Data;
        }

        public class DadosMapeamento
        {
            public string Estado;
            public string Aceita;
        }

        public class Pilha
        {
            public List<String> item;
            override
            public string ToString() {
                String ret = "";
                foreach (String Item in item)
                {
                    ret += Item;
                }
                return ret;

            }
            public Pilha()
            {
                item = new List<String>();
            }
        }


        //=== Encontrar estado na matriz AFD
        static List<int> FindState(string estado, string simbolo, List<AFD> matriz)
        {
            int i = 0, j = 0;
            List<int> index = new List<int>();
            foreach (AFD afd in matriz)
            {
                if (afd.afdCol[0].Equals("*" + estado) || afd.afdCol[0].Equals(estado))
                {
                    j = 0;
                    foreach (string col in matriz[0].afdCol)
                    {
                        if (col.Equals(simbolo))
                        {
                            index.Add(i);
                            index.Add(j);
                            int final = 0;
                            if (afd.afdCol[0].Equals("*" + estado))
                            {
                                final = 1;
                            }
                            index.Add(final);

                            return index;
                        }
                        j++;
                    }
                }
                i++;
            }
            return null;
        }



        //=== Imprimir FITA de saída em CSV
        public static void ImprimirCSV(List<ts> tabela, String nomeArquivo)
        {
            String text = "";

            text += "Linha" + ';';
            text += "Estado" + ';';
            text += "Rotulo" + ';';
            text += "\r\n";

            foreach (ts item in tabela)
            {
                text += item.posicao + ";";
                text += item.estado + ';';
                text += item.rotulo + ';';
                text += "\r\n";
            }

            System.IO.File.WriteAllText(nomeArquivo, text);
        }

        public static Symbol GetSymbol(int Index, List<Symbol> Symbols)
        {
            foreach (var Symbol in Symbols)
            {
                if (Symbol.Index == Index)
                {
                    return Symbol;
                }
            }
            return null;
        }




        static void Main(string[] args)
        {
            //=== Criar tabela de simbolos
            List<ts> tabela = new List<ts>();

            //Buffer
            string buffer;


            //===== Definir arquivos de entrada
            string[] codigo = System.IO.File.ReadAllLines(@"codigo.txt");
            string[] afd = System.IO.File.ReadAllLines(@"AutomatoFinitoEstadoErro.csv");
            string mapeamento = System.IO.File.ReadAllText(@"mapeamentoEstados.txt");

            //=== Carregar AFD em uma matriz
            List<AFD> afdMatriz = new List<AFD>();
            foreach (String linha in afd)
            {
                AFD afdLin = new AFD();
                buffer = "";
                foreach (char c in linha)
                {
                    if (c == ';')
                    {
                        afdLin.afdCol.Add(buffer);
                        buffer = "";
                    }
                    else buffer += c;
                }
                afdMatriz.Add(afdLin);
            }

            //=== Imprimir Matriz do AFD
            foreach (AFD li in afdMatriz)
            {
                foreach (string co in li.afdCol)
                {
                    Console.Write(co + " ");
                }
                Console.WriteLine("");
            }
            //Console.WriteLine("Teste ma[9][4] = " + afdMatriz[9].afdCol[4]);
            //List<int> teste = FindState("L", "6", afdMatriz);
            //Console.WriteLine("Teste ma[L][6] = " + afdMatriz[teste[0]].afdCol[teste[1]]);

            //=== Ler caracter por caracter do codigo e verificar estados no AFD
            string estado = afdMatriz[1].afdCol[0]; //Estado atual - inicial
            string estadoAnt = "";  //Estado anterior
            List<int> indices = new List<int> { 0, 0, 0 };  //Indices da matriz AFD
            buffer = "";
            int pos = 1;    //Contador de posição no arquivo, no caso é a linha do codigo onde esta
            ts tsItem;  //Item da tabela de simbolos

            string Separadores = "/*+-(){},";
            string SeparadoresSemEstado = " \t \n";

            foreach (String linha in codigo)
            {
                if (linha.Length > 0)
                {
                    foreach (char c in linha)
                    {
                        /*Console.WriteLine(c);
                        if (SeparadoresSemEstado.Contains(c))    //Separadores sem estado
                        {
                            if (!buffer.Equals(""))
                            {
                                tsItem = new ts();
                                tsItem.estado = estado;
                                tsItem.posicao = pos;
                                tsItem.rotulo = buffer;
                                buffer = "";
                                tabela.Add(tsItem);
                                estado = afdMatriz[1].afdCol[0]; //Estado atual - inicial
                            }
                        }
                        else if (Separadores.Contains(c))  //Separadores com estado
                        {
                            //Salvar o que tem antes no buffer (se tiver)
                            if (!buffer.Equals(""))
                            {
                                tsItem = new ts();
                                tsItem.estado = estadoAnt;
                                tsItem.posicao = pos;
                                tsItem.rotulo = buffer;
                                tabela.Add(tsItem);
                            }

                            //Agora salva o separador
                            tsItem = new ts();
                            tsItem.estado = estado;
                            tsItem.posicao = pos;
                            tsItem.rotulo = c.ToString();
                            buffer = "";
                            tabela.Add(tsItem);
                            estado = afdMatriz[1].afdCol[0]; //Estado atual - inicial
                        }
                        else
                        {
                            indices = FindState(estado, c.ToString(), afdMatriz);
                            if (indices == null) Console.WriteLine("Erro ao achar estado: (" + estado + ", " + c.ToString() + ")");
                            else
                            {
                                estadoAnt = estado;
                                estado = afdMatriz[indices[0]].afdCol[indices[1]];
                            }
                            buffer += c;
                        }*/


                        if (Separadores.Contains(c))
                        {

                            if (buffer != "")
                            {
                                if (FindState(estado, c.ToString(), afdMatriz) != null) {
                                    if (FindState(estado, c.ToString(), afdMatriz)[2] == 1) // se é um estado final
                                    {
                                        tsItem = new ts();
                                        tsItem.estado = estado;
                                        tsItem.posicao = pos;
                                        tsItem.rotulo = buffer;
                                        buffer = "";
                                        tabela.Add(tsItem);
                                        estado = afdMatriz[1].afdCol[0]; //Estado atual - inicial
                                    }
                                    else
                                    {
                                        buffer += c;
                                    }
                                }
                                else
                                {
                                    {
                                        tsItem = new ts();
                                        tsItem.estado = "!";
                                        tsItem.posicao = pos;
                                        tsItem.rotulo = buffer;
                                        buffer = "";
                                        tabela.Add(tsItem);
                                        estado = afdMatriz[1].afdCol[0]; //Estado atual - inicial
                                    }
                                }
                            }
                            buffer += c;
                            indices = FindState(estado, c.ToString(), afdMatriz);
                            estado = afdMatriz[indices[0]].afdCol[indices[1]];
                        }
                        else
                        {
                            if (SeparadoresSemEstado.Contains(c))
                            {
                                if (buffer != "")
                                {
                                    tsItem = new ts();
                                    tsItem.estado = estado;
                                    tsItem.posicao = pos;
                                    tsItem.rotulo = buffer;
                                    buffer = "";
                                    tabela.Add(tsItem);
                                    estado = afdMatriz[1].afdCol[0]; //Estado atual - inicial
                                }
                            }
                            else
                            {
                                if (FindState(estado, c.ToString(), afdMatriz) != null)
                                {
                                    if (FindState(estado, c.ToString(), afdMatriz)[2] == 1 && (afdMatriz[FindState(estado, c.ToString(), afdMatriz)[0]].afdCol[FindState(estado, c.ToString(), afdMatriz)[1]] == "!"))
                                    {
                                        {
                                            tsItem = new ts();
                                            tsItem.estado = estado;
                                            tsItem.posicao = pos;
                                            tsItem.rotulo = buffer;
                                            buffer = "";
                                            tabela.Add(tsItem);
                                            estado = afdMatriz[1].afdCol[0]; //Estado atual - inicial
                                        }
                                    }
                                }
                                else
                                {
                                    tsItem = new ts();
                                    tsItem.estado = "!";
                                    tsItem.posicao = pos;
                                    tsItem.rotulo = buffer;
                                    buffer = "";
                                    tabela.Add(tsItem);
                                    estado = afdMatriz[1].afdCol[0]; //Estado atual - inicial
                                }

                                indices = FindState(estado, c.ToString(), afdMatriz);
                                estado = afdMatriz[indices[0]].afdCol[indices[1]];

                                buffer += c;

                            }
                        }


                    }


                    if (buffer != "") {
                        //Fim de linha também é separador
                        tsItem = new ts();
                        tsItem.estado = estado;
                        tsItem.posicao = pos;
                        tsItem.rotulo = buffer;
                        buffer = "";
                        tabela.Add(tsItem);
                        estado = afdMatriz[1].afdCol[0]; //Estado atual - inicial
                    }
                }
                pos++;
            }

            tsItem = new ts();
            tsItem.estado = estado;
            tsItem.posicao = pos;
            tsItem.rotulo = "EOF";
            tabela.Add(tsItem);


            //=== Imprimir tabela de simbolos
            foreach (ts item in tabela)
            {
                Console.WriteLine("Linha:" + item.posicao + " Estado:" + item.estado + " Rotulo:" + item.rotulo);
            }

            //=== Imprimir em arquivo CSV
            ImprimirCSV(tabela, "FitaDeSaida.csv");







            //====================== ANÁLISE SINTÁTICA =====================================================================================================


            //=== importa o XML do GOLD Parse
            string xmlData = @"GramaticaTeste.xml";
            XDocument doc = XDocument.Load(xmlData);
            string jsonText = JsonConvert.SerializeXNode(doc);
            dynamic Tables = JsonConvert.DeserializeObject<dynamic>(jsonText);

            //=== Define as estruturas
            List<Symbol> colecaoSimbolos = new List<Symbol>();
            List<Estado> colecaoEstados = new List<Estado>();
            List<Production> colecaoProducoes = new List<Production>();
            List<LALRState> colecaoLALRState = new List<LALRState>();


            //============= Importa os Simbolos do XML

            foreach (var m_Symbol in Tables.Tables.m_Symbol)
            {
                foreach (var Symbols in m_Symbol)
                {
                    if (Symbols.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                    {
                        foreach (var Symbol in Symbols)
                        {
                            Symbol novoSimbolo = new Symbol();

                            foreach (var propriedade in Symbol)
                            {
                                if (propriedade.Name == "@Index")
                                {
                                    string teste = propriedade.Value;
                                    novoSimbolo.Index = Int32.Parse(teste);
                                }
                                if (propriedade.Name == "@Name")
                                {
                                    novoSimbolo.Name = propriedade.Value;
                                }
                            }
                            colecaoSimbolos.Add(novoSimbolo);
                        }
                    }
                }
            }

            //============== Importa os Estados do XML


            foreach (var DFAState in Tables.Tables.DFATable)
            {
                foreach (var estados in DFAState)
                {
                    if (estados.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                    {
                        foreach (var estadoAceitacao in estados)
                        {
                            Estado novoEstado = new Estado();
                            foreach (var propriedade in estadoAceitacao)
                            {
                                if (propriedade.Name == "@AcceptSymbol")
                                {
                                    string valor = propriedade.Value;
                                    novoEstado.AcceptSymbol = GetSymbol(Int32.Parse(valor), colecaoSimbolos);
                                }
                                if (propriedade.Name == "@Index")
                                {
                                    novoEstado.Index = propriedade.Value;
                                }
                            }
                            colecaoEstados.Add(novoEstado);
                        }
                    }
                }
            }

            //=============== Importa as Produções do XML

            foreach (var m_Production in Tables.Tables.m_Production)
            {
                foreach (var Productions in m_Production)
                {
                    if (Productions.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                    {
                        foreach (var Production in Productions)
                        {
                            Production novaProducao = new Production();
                            foreach (var propriedade in Production)
                            {
                                if (propriedade.Name == "@Index")
                                {
                                    string valor = propriedade.Value;
                                    novaProducao.Index = Int32.Parse(valor);
                                }
                                if (propriedade.Name == "@NonTerminalIndex")
                                {
                                    string valor = propriedade.Value;
                                    novaProducao.NonTerminalIndex = GetSymbol(Int32.Parse(valor), colecaoSimbolos);
                                }
                                if (propriedade.Name == "@SymbolCount")
                                {
                                    string valor = propriedade.Value;
                                    novaProducao.SymbolCount = Int32.Parse(valor);
                                }
                                if (propriedade.Name == "ProductionSymbol")
                                {
                                    foreach (var ProductionofProduction in propriedade)
                                    {
                                        foreach (var Producao in ProductionofProduction)
                                        {
                                            foreach (var propriedadeInterna in Producao) {
                                                ProductionSymbol producao = new ProductionSymbol();
                                                string valor = propriedadeInterna.Value;
                                                producao.Symbol = GetSymbol(Int32.Parse(valor), colecaoSimbolos);
                                                novaProducao.ProductionSymbol.Add(producao);
                                            }
                                        }
                                    }
                                }
                            }
                            colecaoProducoes.Add(novaProducao);
                        }
                    }
                }
            }

            //=============== Importa tabela LALR do XML

            foreach (var LALRStates in Tables.Tables.LALRTable)
            {
                foreach (var LALRStatesPropertie in LALRStates)
                {
                    if (LALRStatesPropertie.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                    {
                        foreach (var LALRStateProperties in LALRStatesPropertie)
                        {
                            LALRState novoLALRState = new LALRState();
                            foreach (var LALRStatePropertie in LALRStateProperties)
                            {

                                if (LALRStatePropertie.Name == "@Index")
                                {
                                    string valor = LALRStatePropertie.Value;
                                    novoLALRState.Index = Int32.Parse(valor);
                                }
                                if (LALRStatePropertie.Name == "@ActionCount")
                                {

                                    string valor = LALRStatePropertie.Value;
                                    novoLALRState.ActionCount = Int32.Parse(valor);
                                }
                                if (LALRStatePropertie.Name == "LALRAction")
                                {
                                    if (novoLALRState.ActionCount > 1)
                                    {

                                        foreach (var LALRActions in LALRStatePropertie)
                                        {
                                            foreach (var LALRAction in LALRActions)
                                            {

                                                LALRAction novoLALRAction = new LALRAction();
                                                foreach (var LALRActionPropertie in LALRAction)
                                                {
                                                    if (LALRActionPropertie.Name == "@SymbolIndex")
                                                    {
                                                        string valor = LALRActionPropertie.Value;
                                                        novoLALRAction.SymbolIndex = GetSymbol(Int32.Parse(valor), colecaoSimbolos);
                                                    }
                                                    if (LALRActionPropertie.Name == "@Action")
                                                    {

                                                        string valor = LALRActionPropertie.Value;
                                                        novoLALRAction.Action = Int32.Parse(valor);
                                                    }
                                                    if (LALRActionPropertie.Name == "@Value")
                                                    {
                                                        string valor = LALRActionPropertie.Value;
                                                        novoLALRAction.Value = Int32.Parse(valor);
                                                    }
                                                }
                                                novoLALRState.LALRAction.Add(novoLALRAction);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (var LALRActionProperties in LALRStatePropertie)
                                        {
                                            LALRAction novoLALRAction = new LALRAction();
                                            foreach (var LALRActionPropertie in LALRActionProperties)
                                            {
                                                if (LALRActionPropertie.Name == "@SymbolIndex")
                                                {
                                                    string valor = LALRActionPropertie.Value;
                                                    novoLALRAction.SymbolIndex = GetSymbol(Int32.Parse(valor), colecaoSimbolos);
                                                }
                                                if (LALRActionPropertie.Name == "@Action")
                                                {

                                                    string valor = LALRActionPropertie.Value;
                                                    novoLALRAction.Action = Int32.Parse(valor);
                                                }
                                                if (LALRActionPropertie.Name == "@Value")
                                                {
                                                    string valor = LALRActionPropertie.Value;
                                                    novoLALRAction.Value = Int32.Parse(valor);
                                                }
                                            }
                                            novoLALRState.LALRAction.Add(novoLALRAction);
                                        }
                                    }
                                }
                            }
                            colecaoLALRState.Add(novoLALRState);
                        }
                    }
                }
            }


            //========== Faz o mapeamento dos estados com a fita de saida
            Mapeamento mapeamentoEstados = JsonConvert.DeserializeObject<Mapeamento>(mapeamento);

            foreach (var ts in tabela)
            {




                foreach (var Simbolo in colecaoSimbolos)
                {
                    if (Simbolo.Name == ts.rotulo)
                    {
                        ts.simbolo = Simbolo;
                        break;
                    }
                }
                var temMapeamento = 0;

                if (ts.simbolo == null)
                {

                    foreach (var map in mapeamentoEstados.Data)
                    {

                        if (ts.estado.Substring(0, 1) == map.Estado)
                        {
                            foreach (var Simbolo in colecaoSimbolos)
                            {
                                if (Simbolo.Name == map.Aceita)
                                {
                                    ts.simbolo = Simbolo;
                                    break;
                                }
                            }

                            temMapeamento = 1;
                        }
                    }
                }




                foreach (var Estado in colecaoEstados)
                {
                    if (Estado.AcceptSymbol == ts.simbolo)
                    {
                        ts.estadoGLC = Estado;
                        break;
                    }
                }
            }



            Pilha pilha = new Pilha(){};

            pilha.item.Add("0");

            var aceita = 0;
            int posicao = 0;
            int erro = 1;
            while (aceita == 0)
            {
                erro = 1;
                foreach (var LALRState in colecaoLALRState)
                {
                    if (LALRState.Index == Int32.Parse(pilha.item[pilha.item.Count - 1]))
                    {
                        foreach (var LALRAction in LALRState.LALRAction)
                        {
                            if (LALRAction.SymbolIndex.Index == tabela[posicao].simbolo.Index)
                            {
                                erro = 0;
                                switch (LALRAction.Action)
                                {
                                    case 1: // Salto     
                                        pilha.item.Add(tabela[posicao].rotulo);
                                        pilha.item.Add(LALRAction.Value.ToString());
                                        posicao++;
                                        Console.WriteLine(pilha.ToString());
                                        break;
                                    case 2: // Reducao
                                        // busca o tamanho da producao
                                        

                                        var ProducaoDaReducao = LALRAction.Value;
                                        var SimboloNaoTerminal = colecaoProducoes[ProducaoDaReducao].NonTerminalIndex;

                                        var tamanho = colecaoProducoes[ProducaoDaReducao].SymbolCount * 2;

                                        pilha.item.RemoveRange(pilha.item.Count - tamanho, tamanho);

                                        foreach (var LALRStateAux in colecaoLALRState)
                                        {
                                            if (LALRStateAux.Index == Int32.Parse(pilha.item[pilha.item.Count - 1]))
                                            {
                                                foreach (var LALRActionAux in LALRStateAux.LALRAction)
                                                {
                                                    if (LALRActionAux.SymbolIndex == SimboloNaoTerminal)
                                                    {
                                                        pilha.item.Add(SimboloNaoTerminal.Name);
                                                        pilha.item.Add(LALRActionAux.Value.ToString());
                                                        break;
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                        Console.WriteLine(pilha.ToString());
                                        break;
                                    case 3: // Empilhamento
                                        pilha.item.Add(tabela[posicao].simbolo.Name);
                                        pilha.item.Add(LALRAction.Value.ToString());
                                        posicao++;
                                        Console.WriteLine(pilha.ToString());
                                        break;
                                    case 4:
 /*                                       ProducaoDaReducao = LALRAction.Value;
                                        SimboloNaoTerminal = colecaoProducoes[ProducaoDaReducao].NonTerminalIndex;
                                        */
                                        aceita = 1;

 /*                                       tamanho = 2;

                                        pilha.item.RemoveRange(pilha.item.Count - tamanho, tamanho);
                                        posicao++;
                                        Console.WriteLine(pilha.ToString());*/
                                        break;
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
                if (erro == 1)
                {
                    Console.WriteLine("O Analisador sintático encontrou um erro na linha: " + tabela[posicao].posicao);
                    break;
                }
            }

            if (erro == 0)
            {
                Console.WriteLine("Análise Léxica concluída com sucesso!");
            }


            //===================== ANALISADOR SEMÂNTICO ==============================


        }
    }
}