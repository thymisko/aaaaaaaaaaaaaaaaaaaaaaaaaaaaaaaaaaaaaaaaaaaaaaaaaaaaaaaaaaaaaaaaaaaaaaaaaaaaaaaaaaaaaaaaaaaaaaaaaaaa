using System;
using System.Collections.Generic;
using System.IO;

namespace a {
  internal class Program {
    static readonly Dictionary < string, char > AArchMap = new() {
      ["A"] = '>',
      ["a"] = '<',
      ["Aa"] = '+',
      ["aA"] = '-',
      ["AA"] = '.',
      ["aa"] = ',',
      ["Aaa"] = '[',
      ["aAA"] = ']'
    };

    static void Main(string[] args) {
      if (args.Length >= 3 && args[0] == "/run" && args[1] == "/file") {
        string path = args[2];
        if (!File.Exists(path)) {
          Console.WriteLine($"File not found: {path}");
          return;
        }
        
        string code = File.ReadAllText(path);
        string bfCode = ConvertToBrainfuck(code);
        RunBrainfuck(bfCode);
      } else {
        Console.WriteLine("Usage: abf /run /file C:\\Path\\To\\File.aaa");
      }
    }

    static string ConvertToBrainfuck(string code) {
      var bf = new List < char > ();
      var lines = code.Split(new [] {
        "\r\n",
        "\n"
      }, StringSplitOptions.None);

      foreach(var rawLine in lines) {
        var line = rawLine.Trim();
        qif(line.StartsWith(";")) continue; // full-line comment
        // remove inline comments (optional)
        int commentIndex = line.IndexOf(';');
        if (commentIndex != -1) {
          line = line.Substring(0, commentIndex).Trim();
        }
        for (int i = 0; i < line.Length;) {
          bool matched = false;
          for (int len = 3; len >= 1; len--) {
            if (i + len <= line.Length) {
              string segment = line.Substring(i, len);
              if (AArchMap.TryGetValue(segment, out char bfChar)) {
                bf.Add(bfChar);
                i += len;
                matched = true;
                break;
              }
            }
          }
          if (!matched) i++; // skip anything not matching
        }
      }

      return new string(bf.ToArray());
    }

    static void RunBrainfuck(string code) {
      byte[] tape = new byte[30000];
      int ptr = 0;
      var loopStack = new Stack < int > ();

      for (int i = 0; i < code.Length; i++) {
        char cmd = code[i];
        switch (cmd) {
        case '>':
          ptr++;
          break;
        case '<':
          ptr--;
          break;
        case '+':
          tape[ptr]++;
          break;
        case '-':
          tape[ptr]--;
          break;
        case '.':
          Console.Write((char) tape[ptr]);
          break;
        case ',':
          tape[ptr] = (byte) Console.Read();
          break;
        case '[':
          if (tape[ptr] == 0) {
            int open = 1;
            while (open > 0 && ++i < code.Length) {
              if (code[i] == '[') open++;
              else if (code[i] == ']') open--;
            }
          } else loopStack.Push(i);
          break;
        case ']':
          if (tape[ptr] != 0) {
            i = loopStack.Peek();
          } else loopStack.Pop();
          break;
        }
      }
    }
  }
}
