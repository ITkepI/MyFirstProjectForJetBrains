using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JetBrainsProject
{
    internal class RPN
    {
        static public string Calculate(string input)
        {
            string output = GetExpression(input);
            string result = Counting(output);
            return result;
        }

        //Метод, преобразующий входную строку с выражением в постфиксную запись
        static private string GetExpression(string input)
        {
            string output = string.Empty;
            Stack<char> operStack = new Stack<char>(); //Стек для хранения операторов

            for (int i = 0; i < input.Length; i++)
            {
                if (IsDelimeter( input[i] ))
                    continue;

                if (Char.IsDigit( input[i] ) || Char.IsLetter( input[i] ))
                {
                    while (!IsDelimeter( input[i] ) && !IsOperator( input[i] ))
                    {
                        output += input[i];
                        i++;

                        if (i == input.Length) break;
                    }

                    output += " ";
                    i--;
                }

                if (IsOperator( input[i] ))
                {
                    if (input[i] == '(')
                        operStack.Push( input[i] );
                    else if (input[i] == ')')
                    {
                        char s = operStack.Pop();

                        while (s != '(')
                        {
                            output += s.ToString() + " ";
                            s = operStack.Pop();
                        }
                    }
                    else
                    {
                        if (operStack.Count > 0) //Если в стеке есть элементы
                            if (GetPriority( input[i] ) <= GetPriority( operStack.Peek() )) //И если приоритет нашего оператора меньше или равен приоритету оператора на вершине стека
                                output += operStack.Pop().ToString() + " "; //То добавляем последний оператор из стека в строку с выражением

                        operStack.Push( char.Parse( input[i].ToString() ) ); //Если стек пуст, или же приоритет оператора выше - добавляем операторов на вершину стека
                    }
                }
            }

            //Когда прошли по всем символам, выкидываем из стека все оставшиеся там операторы в строку
            while (operStack.Count > 0)
                output += operStack.Pop() + " ";

            return output;
        }

        //Метод, вычисляющий значение выражения, уже преобразованного в постфиксную запись
        static private String Counting(string input)
        {
            Stack<string> temp = new Stack<string>();

            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsDigit( input[i] ) || Char.IsLetter( input[i] ))
                {
                    string a = string.Empty;

                    while (!IsDelimeter( input[i] ) && !IsOperator( input[i] ))
                    {
                        a += input[i];
                        i++;
                        if (i == input.Length) break;
                    }
                    temp.Push( a );
                    i--;
                }
                else if (IsOperator( input[i] ))
                {
                    string a = temp.Pop();
                    string b = temp.Pop();

                    if (Char.IsLetter( a, 0 ) && Char.IsLetter( b, 0 )) //Из условия я посчитал, что переменные могут быть только однобуквенные
                    {
                        switch (input[i])
                        {
                            case '+': temp.Push( b + " + " + a ); break;
                            case '-': temp.Push( b + " - " + a ); break;
                            case '*': temp.Push( b + " * " + a ); break;
                            case '/': temp.Push( b + " / " + a ); break;
                        }
                    }
                    else if (!Char.IsLetter( a, 0 ) && Char.IsLetter( b, 0 ))
                    {
                        switch (input[i])
                        {
                            case '+': temp.Push( b + " + " ); break;
                            case '-': temp.Push( b + " - " ); break;
                            case '*': temp.Push( b + " * " ); break;
                            case '/': temp.Push( b + " / " ); break;
                        }
                        temp.Push( a );
                    }
                    else if (Char.IsLetter( a, 0 ) && !Char.IsLetter( b, 0 ))
                    {
                        switch (input[i])
                        {
                            case '+': temp.Push( " + " + a ); break;
                            case '-': temp.Push( " - " + a ); break;
                            case '*': temp.Push( " * " + a ); break;
                            case '/': temp.Push( " / " + a ); break;
                        }
                        temp.Push( b );
                    }
                    else
                    {
                        int x = Convert.ToInt32(a);
                        int y = Convert.ToInt32(b);
                        int z;

                        switch (input[i])
                        {
                            case '+': temp.Push( (y + x).ToString() ); break;
                            case '-': temp.Push( (y - x).ToString() ); break;
                            case '*': temp.Push( (y * x).ToString() ); break;
                            case '/': //из условия я посчитал, что не только в начальном, но и в сокращённом выражении должны быть целочисленные значения
                                Math.DivRem( y, x, out z );
                                if (z == 0)
                                {
                                    temp.Push( (y / x).ToString() );
                                    break;
                                }
                                else
                                {
                                    temp.Push( (b + " / " + a).ToString() );
                                    break;
                                }
                        }
                    }
                }
            }
            string t1 = temp.Pop();
            string t2 = temp.Peek();
            return t2 + t1; //Забираем результат всех вычислений из стека и возвращаем его
        }

        //Метод возвращает true, если проверяемый символ - разделитель ("пробел" или "равно")
        static private bool IsDelimeter(char c)
        {
            if (c == ' ')
            {
                return true;
            }

            return false;
        }

        //Метод возвращает true, если проверяемый символ - оператор
        static private bool IsOperator(char c)
        {
            if ("+-/*()".Contains( c ))
            {
                return true;
            }

            return false;
        }

        //Метод возвращает приоритет оператора
        static private byte GetPriority(char s)
        {
            switch (s)
            {
                case '(': return 0;
                case ')': return 1;
                case '+': return 2;
                case '-': return 3;
                case '*': return 4;
                case '/': return 4;
                default: return 5;
            }
        }
    }

    internal class Program
    {
        private static void Main(string[ ] args)
        {
            while (true) //Бесконечный цикл
            {
                Console.Write( "Введите выражение: " );
                Console.WriteLine( RPN.Calculate( Console.ReadLine() ) );
            }
        }
    }
}
