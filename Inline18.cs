using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;


namespace Inline2018
{
    class Inline18
    {
        Dictionary<string, string> variables = new Dictionary<string, string>();
        List<string> list = new List<string>();
        List<string> output = new List<string>();
        bool isLive;
        
        SyntaX syntax = new SyntaX();
        MyParser expPars = new MyParser();
        public Inline18(bool live)
        {
            isLive = live;
        }
        bool IsVarDeclared(string var)
        {
            return variables.ContainsKey(var);
        }
        string VarReturn(string input)
        {
            string oldStr, newStr, varName;
            Regex varRex = new Regex(@"\[(?'var'[A-z]+[0-9]*[_\.\$#]*?)\]");
            while (varRex.IsMatch(input))
            {
                oldStr = varRex.Match(input).Value.ToString();
                varName = varRex.Match(input).Groups["var"].ToString();
                newStr = variables[varName].ToString();
                if (IsVarDeclared(varName))
                    input = input.Replace(oldStr, newStr);
                else
                {
                    //Console.WriteLine("Variable " + varName + " not declared"); return "";
                }
            }
            return input;
        }
        string VarOutput(string input)
        {
            string oldStr, newStr, varName;
            Regex varRex = new Regex(@"=\[(?'var'.*?)\]");

            while (varRex.IsMatch(input))
            {

                oldStr = varRex.Match(input).Value.ToString();
                varName = varRex.Match(input).Groups["var"].ToString();
                newStr = variables[varRex.Match(input).Groups["var"].ToString()].ToString();

                if (IsVarDeclared(varName))
                    input = input.Replace(oldStr, newStr);
                else
                {// Console.WriteLine("Variable " + varName + " not declared");
                    return ""; }

            }
            input = input.Replace("\"", "");
            return input;
        }
        string DeclareVar(string input,bool live)
        {

            Regex decRex = new Regex(@"^\[(?'var'.*)\] *= *(?'rest'[^\}]*)");
            string varName = decRex.Match(input).Groups["var"].ToString();
            string restLine = decRex.Match(input).Groups["rest"].ToString();

            restLine = VarReturn(restLine);

            if (!IsVarDeclared(varName))
            {
                variables.Add(varName, expPars.parseString(restLine));
                if (live)
                    list.Add("<< Variable " + varName + " declared with value" + variables[varName]+" >>\n");
            }
            else
            { variables[varName] = expPars.parseString(restLine);
                if (live)
                    list.Add("<< Variable " + varName + " changed value to" + variables[varName]+" >>\n");
            }

            
            return variables[varName];
        }
        string InlineBlock(string input,bool live)
        {
            Regex inRex = new Regex(@"@\{(?'inline'.*)\}");
            string oldStr, newStr;
            while (inRex.IsMatch(input))
            {
                oldStr = inRex.Match(input).Value.ToString();
                newStr = inRex.Match(input).Groups["inline"].ToString();
                input = input.Replace(oldStr, "");
                DeclareVar(newStr,live);
            }
            return input;
        }
        async Task<List<string>> IncludeFile(string input,bool live)
        
        {
            Regex incRex = new Regex(@"include *(?'path'.+\.txt)");
            string oldStr, newStr;
           
                oldStr = incRex.Match(input).Value.ToString();
                newStr = incRex.Match(input).Groups["path"].ToString();
                try
                {

                    Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.LocalFolder;
                    Windows.Storage.StorageFile sampleFile = await storageFolder.GetFileAsync(newStr);                   
                    var text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
                if(live)
                list.Add("<< File" + newStr + " included >>\n");
                    int index = list.IndexOf(input);
                    list =await  Run(text.Split('\r'));
                    input = input.Replace(oldStr, "");
                    return list;
                }
                catch (Exception e)
                {

                if (live)
                    list.Add("<< File " + newStr + " invalid >>\n");
                    return list;
                }
                
            
        }
        string Tags(string input )
        {
            Regex tagRex = new Regex(@"<all_caps>+(?'tag'.*)</all_caps>+");
            string oldStr, newStr;
            while (tagRex.IsMatch(input))
            {
                oldStr = tagRex.Match(input).Value.ToString();
                newStr = tagRex.Match(input).Groups["tag"].ToString();
                input = input.Replace(oldStr, newStr.ToUpper());
            }
            return input;
        }


        public async Task<List<string>> Run(string[] lines)
        {
            
                foreach (var line in lines)
                {
                if (!syntax.IsValidLine(line))//works
                {
                    if (isLive)
                        list.Add("<< Line is not valid >>\n");
                    return list;
                }

                    if (syntax.IsValidPath(line))//works
                       list=await IncludeFile(line,isLive );
                    if (syntax.IsDeclaration(line))
                        DeclareVar(line,isLive);



                    // output.Add(line);
                    if (!syntax.IsDeclaration(line) && !syntax.IsValidPath(line))
                        list.Add(Tags(VarOutput(InlineBlock(line,isLive))));//works
                }
            
            
            return list;
        }

    }
}
