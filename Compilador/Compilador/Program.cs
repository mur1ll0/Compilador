using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compilador
{
    class Program
    {

        //===ESTRUTURAS DO PROJETO
        public class Estado
        {
            public String nome;
            public Boolean final;

            public Estado(){
                final = false;
            }

        }

        public class Producao
        {
            public Char simboloT;
            public Estado simboloNT;
            public Boolean valida;

            public Producao() {
                simboloNT = new Estado();
                valida = true;
            }
        }

        public class Regra
        {
            public Estado nomeRegra;
            public List<Producao> producoes;
            public Boolean valida;

            public Regra()
            {
                nomeRegra = new Estado();
                producoes = new List<Producao>();
                valida = true;
            }
        }

        //===FUNCOES DO PROJETO
        static void Main(string[] args)
        {
            //Declarar estruturas internas
            List<Regra> gramaticaRegular = new List<Regra>();   //Lista de regras que a gramatica contem
            List<String> tokens = new List<String>();   //Lista de tokens do arquivo de entrada
            List<Char> alfabeto = new List<Char>(); //Lista de simbolos terminais do arquivo de entrada

            String estadoInicial = "S";

            //===== Leitura do arquivo

            string[] linhasArquivo = System.IO.File.ReadAllLines(@"Entrada.txt");

            //====== Define os tokens
            {
                foreach (String linha in linhasArquivo)
                {
                    if ((linha.Length > 0) && (!linha.Contains("::=")))
                    {
                        tokens.Add(linha);
                    }

                }

            }

            //====== Define os Estados 
            {
                foreach (String linha in linhasArquivo)
                {
                    if (linha.Length > 0)
                    {
                        //Percorre somente se a linha tiver a sintaxe de gramatica
                        if (linha.Contains("::="))
                        {
                            String buffer = "";
                            int state = 0;
                            int prodCount = 0;
                            Regra regra = new Regra();

                            //Percorrer caracter por caracter
                            foreach (char c in linha)
                            {
                                switch (c)
                                {
                                    case ':':
                                        if (state == 0) state = 1;  //Estado reconheceu 1 ':'
                                        else if (state == 1) state = 2;  //Estado reconheceu 2 ':'
                                        else
                                        {
                                            //É um simbolo terminal
                                            Producao producao = new Producao();
                                            producao.simboloT = buffer[0];
                                            regra.producoes.Add(producao);
                                            prodCount++;
                                            state = 4;  //Estado reconheceu simbolo terminal
                                        }
                                        break;

                                    case '=':
                                        if (state == 2)
                                        {  //Reconheceu simbolo que da nome à regra
                                            Estado estado = new Estado();
                                            estado.nome = buffer;
                                            regra.nomeRegra = estado;

                                            buffer = "";
                                            state = 3;  //Estado reconheceu Regra
                                        }
                                        else
                                        {
                                            //É um simbolo terminal
                                            Producao producao = new Producao();
                                            producao.simboloT = buffer[0];
                                            regra.producoes.Add(producao);
                                            prodCount++;
                                            state = 4;  //Estado reconheceu simbolo terminal
                                        }
                                        break;

                                    case '|':
                                        if (state == 3)
                                        {
                                            //Reconheceu apenas símbolo terminal, com Epsilon transição
                                            Producao producao = new Producao();
                                            //producao.simboloT = Char.Parse(buffer);
                                            producao.simboloT = buffer[0];
                                            regra.producoes.Add(producao);
                                            prodCount++;
                                        }
                                        break;

                                    case '<':
                                        if (buffer.Length > 0)   //Caso tenha valor no buffer, tratar como simbolo terminal
                                        {
                                            Producao producao = new Producao();
                                            producao.simboloT = buffer[0];
                                            regra.producoes.Add(producao);
                                            prodCount++;
                                            state = 4;  //Estado reconheceu simbolo terminal
                                        }
                                        buffer = "";
                                        break;

                                    case '>':
                                        if (state == 4)
                                        {
                                            //Reconheceu simbolo não terminal
                                            Estado simboloNT = new Estado();
                                            simboloNT.nome = buffer;
                                            regra.producoes[prodCount-1].simboloNT = simboloNT;
                                            buffer = "";
                                        }
                                        if (state == 3)
                                        {
                                            //Reconheceu apenas símbolo não terminal (Produção Unitária)
                                            Producao producao = new Producao();
                                            Estado simboloNT = new Estado();
                                            simboloNT.nome = buffer;
                                            producao.simboloNT = simboloNT;
                                            regra.producoes.Add(producao);
                                            prodCount++;
                                            buffer = "";
                                        }
                                        break;

                                    case ' ':
                                        //Se for um espaço, ignora
                                        break;

                                    default:
                                        buffer += c;
                                        break;
                                }
                            }
                            //Adicionar regra na gramática
                            gramaticaRegular.Add(regra);
                        }
                    }
                }
            }

            //===Imprimir regras
            foreach(Regra regra in gramaticaRegular)
            {
                Console.Write(regra.nomeRegra.nome + " ::= ");
                foreach(Producao prod in regra.producoes)
                {
                    Console.Write($"{prod.simboloT}<{prod.simboloNT.nome}>|");
                }
                Console.WriteLine("");
            }

        }
    }
}
