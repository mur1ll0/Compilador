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
            Console.WriteLine("ma[9][4] = " + afdMatriz[9].afdCol[4]);

            //=== Ler caracter por caracter do codigo e verificar estados no AFD
            foreach (String linha in codigo)
            {

            }
        }
    }
}
