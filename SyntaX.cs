using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Inline2018
{
    
    
        public class SyntaX
        {
            char[] slova = {'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q',
                        'R','S','T','U','V','W','X','Y','Z','a','b','c','d','e','f','g','h',
                        'i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z' };
            char[] brojevi = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            char[] operatori = { '+', '-', '*', '/', '%' };
            char[] spec_karakteri = { '#', '_', '.', '$', '@', '{', '}', '[', ']', '(', ')', '=', '<', '>', ' ', '"', ':', '\\' };
            public bool IsBalanced(string a, string b, string input)
            {
                Stack<char> stack = new Stack<char>();
                string str = input;
                char x = '~';
                char y = '^';
                str = str.Replace(a, x.ToString());

                str = str.Replace(b, y.ToString());
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == x)
                    {
                        stack.Push(str[i]); continue;
                    }
                    if (str[i] == y)
                        if (stack.Count == 0)
                            return false;
                        else
                            stack.Pop();

                }
                return stack.Count == 0;
            }
            public bool IsValidCharacters(string input)
            {

                foreach (var x in input)
                {
                    if (operatori.Contains(x) || slova.Contains(x) || brojevi.Contains(x) || spec_karakteri.Contains(x))
                        continue;
                    else
                    {
                       // Console.Write(x + ":");
                        return false;
                    }
                }
                return true;
            }
            public bool IsValidPath(string input)
            {

                return Regex.IsMatch(input, @"include *(?'path'.+\.txt)");
            }
            public bool IsValidString(string input)
            {
                return input.Count(x => x == '"') % 2 == 0;
            }
            public bool IsDeclaration(string input)
            {
                return Regex.IsMatch(input, @"^\[(?'var'.*)\] *= *(?'rest'.*)$");
            }

            public bool IsValidLine(string input)
            {
                if (!IsBalanced("(", ")", input))
                {
                    //Console.WriteLine("Parenthesis not closed");
                    return false;
                }
                if (!IsBalanced("<all_caps>", "/all_caps", input))
                {
                   // Console.WriteLine("Tags not closed");
                    return false;
                }
                if (!IsBalanced("{", "}", input))
                {
                  //  Console.WriteLine("Inline block not closed");
                    return false;
                }
                if (!IsValidString(input))
                {
                  //  Console.WriteLine("String invalid");
                    return false;
                }
                if (!IsValidCharacters(input))
                {
                  //  Console.WriteLine("Invalid character used");
                    return false;
                }
                return true;

            }
        }
    }

