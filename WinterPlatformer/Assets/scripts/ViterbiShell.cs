using System.Collections;
using System.Collections.Generic;
using System.IO;
using com.cozyhome.Console;
using UnityEngine;

public class ViterbiShell : MonoBehaviour
{
    private Dictionary<string, Dictionary<string, int>> uni_freq;
    private Dictionary<string, int>                     uni_freq_s;
    
    private Dictionary<string, Dictionary<string, int>> bi_frq;
    private Dictionary<string, int>                     bi_freq_s;

    void Start() {
        MonoConsole.InsertCommand("viterbi_tag", (string[] modifiers, out string output) => {

            output = "================";
            MonoConsole.PrintToScreen("exec() viterbi_tag");
            
            if(modifiers == null || string.IsNullOrEmpty(modifiers[0])) {
                MonoConsole.PrintToScreen("error: did not provide filepath");
                return;
            }
            else {
                string fp = Application.dataPath + "/Resources/" + modifiers[0];
                MonoConsole.PrintToScreen($"parsing {fp}");
                //sr.ReadLine().Split();
                // begin reading via streamwriter
                using(StreamReader sr = new StreamReader(fp)) {                    
                    while(!sr.EndOfStream) {
                        string[] tuple = System.Text.RegularExpressions.Regex.Split(sr.ReadLine(), " +");
                        if(tuple.Length == 2) {
                            // MonoConsole.PrintToScreen($"{tuple[0]}, {tuple[1]}");
                            
                        }
                        else {
                            // MonoConsole.PrintToScreen($"<empty>");
                        }
                    }
                }
            }
        });
    }
}