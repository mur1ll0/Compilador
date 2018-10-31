﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        

        //=== Encontrar estado na matriz AFD
        static List<int> FindState(string estado, string simbolo, List<AFD> matriz)
        {
            int i = 0, j = 0;
            List<int> index = new List<int>();
            foreach (AFD afd in matriz)
            {
                if (afd.afdCol[0].Equals("*"+estado) || afd.afdCol[0].Equals(estado))
                {
                    j = 0;
                    foreach(string col in matriz[0].afdCol)
                    {
                        if (col.Equals(simbolo))
                        {
                            index.Add(i);
                            index.Add(j);
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


        static void Main(string[] args)
        {
            //=== Criar tabela de simbolos
            List<ts> tabela = new List<ts>();

            //Buffer
            string buffer;


            //===== Definir arquivos de entrada
            string[] codigo = System.IO.File.ReadAllLines(@"codigo.txt");
            string[] afd = System.IO.File.ReadAllLines(@"AutomatoFinitoEstadoErro.csv");

            //=== Carregar AFD em uma matriz
            List<AFD> afdMatriz = new List<AFD>();
            foreach (String linha in afd)
            {
                AFD afdLin = new AFD();
                buffer = "";
                foreach(char c in linha)
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
            foreach(AFD li in afdMatriz)
            {
                foreach(string co in li.afdCol)
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
            List<int> indices;  //Indices da matriz AFD
            buffer = "";
            int pos = 0;    //Contador de posição no arquivo, no caso é a linha do codigo onde esta
            ts tsItem;  //Item da tabela de simbolos
            foreach (String linha in codigo)
            {
                if(linha.Length > 0)
                {
                    foreach (char c in linha)
                    {
                        Console.WriteLine(c);
                        if (c == ' ')    //Separadores sem estado
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
                        else if (c == '*' || c == '/' || c == '+' || c == '-' || c == '(' || c == ')')  //Separadores com estado
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
                        }
                    }

                    //Fim de linha também é separador
                    tsItem = new ts();
                    tsItem.estado = estado;
                    tsItem.posicao = pos;
                    tsItem.rotulo = buffer;
                    buffer = "";
                    tabela.Add(tsItem);
                    estado = afdMatriz[1].afdCol[0]; //Estado atual - inicial
                }
                pos++;
            }

            //=== Imprimir tabela de simbolos
            foreach(ts item in tabela)
            {
                Console.WriteLine("Linha:" + item.posicao + " Estado:" + item.estado + " Rotulo:" + item.rotulo);
            }

            //=== Imprimir em arquivo CSV
            ImprimirCSV(tabela, "FitaDeSaida.csv");
        }
    }
}