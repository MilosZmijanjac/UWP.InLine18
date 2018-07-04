using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Inline2018
{
    class MyParser
    {

        int pos = -1, ch;
        string str;

        SyntaX a = new SyntaX();
        void nextChar()
        {
            ch = (++pos < str.Length) ? str.ElementAt(pos) : -1;

        }

        bool eat(int charToEat)
        {
            while (ch == ' ') nextChar();
            if (ch == charToEat)
            {

                nextChar();
                return true;
            }
            return false;
        }

        public double parse(string s)
        {

            str = s;
            pos = -1; ch = 0;
            nextChar();
            double x = parseExpression();
            if (pos < str.Length) throw new Exception("Unexpected: " + (char)ch);
            return x;
        }
        double parseExpression()
        {
            double x = parseTerm();
            for (;;)
            {
                if (eat('+')) x += parseTerm(); // addition
                else if (eat('-')) x -= parseTerm(); // subtraction
                else return x;
            }
        }

        double parseTerm()
        {
            double x = parseFactor();
            for (;;)
            {
                if (eat('*')) x *= parseFactor(); // multiplication
                else if (eat('/')) x /= parseFactor();// division
                else if (eat('%')) x %= parseFactor();// mod
                else return x;
            }
        }
        double parseFactor()
        {
            if (eat('+')) return parseFactor(); // unary plus
            if (eat('-')) return -parseFactor(); // unary minus

            double x;
            int startPos = this.pos;
            if (eat('('))
            { // parentheses
                x = parseExpression();
                eat(')');
            }
            else if ((ch >= '0' && ch <= '9') || ch == '.')
            { // numbers
                while ((ch >= '0' && ch <= '9') || ch == '.') nextChar();
                string pom = str.Substring(startPos, this.pos - startPos);
                x = Double.Parse(pom);
                

            }
            else if (ch >= 'a' && ch <= 'z')
            { // functions
                while (ch >= 'a' && ch <= 'z') nextChar();
                string func = str.Substring(startPos, this.pos - startPos);
                x = parseFactor();
                if (func.Equals("sqrt")) x = Math.Sqrt(x);
                else if (func.Equals("sin")) x = Math.Sin((x));
                else if (func.Equals("cos")) x = Math.Cos((x));
                else if (func.Equals("tan")) x = Math.Tan((x));
                else throw new Exception("Unknown function: " + func);
            }

            else
            {
                throw new Exception("Unexpected: " + (char)ch);
            }

            if (eat('^')) x = Math.Pow(x, parseFactor()); // exponentiation

            return x;
        }

        public string parseString(string input)
        {

            input = input.Replace(" ", "");
            Regex literal_str = new Regex("\".*?\"");
            Regex zagrade = new Regex(@"\((?'sadrzaj'.*?)\)");
            if (literal_str.IsMatch(input))
            {
                while (zagrade.IsMatch(input))
                {
                    string x = zagrade.Match(input).Value;
                    int indexX = zagrade.Match(input).Index + x.Length;
                    int indexY = zagrade.Match(input).Index + x.Length - 1;
                    string y = zagrade.Match(input).Groups["sadrzaj"].ToString();


                    while (!a.IsBalanced("(", ")", x))
                    {
                        y += input[indexY++];
                        x += input[indexX++];
                    }

                    //   Console.WriteLine(x.Last()+"-"+x.Length);
                    //   Console.WriteLine("x:" + x + "y:" + y);
                    if (literal_str.IsMatch(zagrade.Match(input).Groups["sadrzaj"].ToString()))
                    {
                        // Console.WriteLine(1);
                        input = input.Replace(x, y);
                        // Console.WriteLine("input1:" + input);
                    }
                    else
                    {
                        // Console.WriteLine(2);
                        input = input.Replace(x, parse(y).ToString());
                        //Console.WriteLine("input2:" + input);
                    }

                }
                input = input.Replace("+", "");
                input = input.Replace("\"", "");
                input = input.Insert(0, "\"");
                input = input.Insert(input.Length, "\"");
            }
            else
            {
                // Console.WriteLine(3);
                input = parse(input).ToString();
                // Console.WriteLine("input3:" + input);
            }
            return input;
        }
        bool IsBalanced(char a, char b, string k)
        {
            return k.Count(x => x == a) == k.Count(x => x == b);
        }

    }
}

