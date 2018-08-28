using System;
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
            String rotulo;
            String estado;
            int posicao;
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


        static void Main(string[] args)
        {
            //=== Criar tabela de simbolos
            List<ts> tabela = new List<ts>();


            //===== Definir arquivos de entrada
            string[] codigo = System.IO.File.ReadAllLines(@"codigo.txt");
            string[] afd = System.IO.File.ReadAllLines(@"AutomatoFinitoEstadoErro.csv");

            //=== Carregar AFD em uma matriz
            List<AFD> afdMatriz = new List<AFD>();
            foreach (String linha in afd)
            {
                AFD afdLin = new AFD();
                string buffer = "";
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
            Console.WriteLine("Teste ma[9][4] = " + afdMatriz[9].afdCol[4]);
            List<int> teste = FindState("L", "6", afdMatriz);
            Console.WriteLine("Teste ma[L][6] = " + afdMatriz[teste[0]].afdCol[teste[1]]);

            //=== Ler caracter por caracter do codigo e verificar estados no AFD
            foreach (String linha in codigo)
            {

            }
        }
    }
}
