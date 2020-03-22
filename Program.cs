/*
** Jo Sega Saturn Engine
** Copyright (c) 2012-2020, Johannes Fetz (johannesfetz@gmail.com)
** All rights reserved.
**
** Redistribution and use in source and binary forms, with or without
** modification, are permitted provided that the following conditions are met:
**     * Redistributions of source code must retain the above copyright
**       notice, this list of conditions and the following disclaimer.
**     * Redistributions in binary form must reproduce the above copyright
**       notice, this list of conditions and the following disclaimer in the
**       documentation and/or other materials provided with the distribution.
**     * Neither the name of the Johannes Fetz nor the
**       names of its contributors may be used to endorse or promote products
**       derived from this software without specific prior written permission.
**
** THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
** ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
** WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
** DISCLAIMED. IN NO EVENT SHALL Johannes Fetz BE LIABLE FOR ANY
** DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
** (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
** LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
** ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
** (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
** SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace JoEngineCueMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Jo Engine Cuesheet Maker v1.1 (c) 2020 by Johannes Fetz");
                Console.WriteLine();

                string folderPath = args.Length > 0 ? args[0] : Environment.CurrentDirectory;
                string cueFilePath = Program.GenerateCue(folderPath);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} generated.", Path.GetFileName(cueFilePath)));
            }
            catch (Exception ex)
            {
                Program.Error(ex.Message);
            }
            finally
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static string GenerateCue(string folderPath)
        {
            string isoFilePath = Program.GetIsoFilePath(folderPath);
            int track = 1;
            StringBuilder cue = new StringBuilder();
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} found.", Path.GetFileName(isoFilePath)));
            Program.GenerateCueHeader(cue, isoFilePath, ref track);
            foreach (string file in Directory.GetFiles(folderPath, "*.*"))
                Program.AppendFileToCue(cue, file, ref track);
            return Program.ExportCueToFile(cue, folderPath, isoFilePath);
        }

        private static string GetIsoFilePath(string path)
        {
            string[] isos = Directory.GetFiles(path, "*.iso");
            if (isos == null || isos.Length <= 0)
                Program.Error(string.Format(CultureInfo.InvariantCulture, "No ISO found in path: {0}", path));
            if (isos.Length > 1)
                Program.Error(string.Format(CultureInfo.InvariantCulture, "Multiple ISO found in path: {0}", path));
            return isos[0];
        }

        private static void GenerateCueHeader(StringBuilder cue, string isoFilePath, ref int track)
        {
            cue.AppendLine(string.Format(CultureInfo.InvariantCulture, "FILE \"{0}\" BINARY", Path.GetFileName(isoFilePath)));
            cue.AppendLine(string.Format(CultureInfo.InvariantCulture, "  TRACK {0:D2} MODE1/2048", track++));
            cue.AppendLine("      INDEX 01 00:00:00");
            cue.AppendLine("      POSTGAP 00:02:00");
        }

        private static void AppendFileToCue(StringBuilder cue, string file, ref int track)
        {
            string ext = Path.GetExtension(file).ToLowerInvariant();
            if (ext == ".mp3")
                cue.AppendLine(string.Format(CultureInfo.InvariantCulture, "FILE \"{0}\" MP3", Path.GetFileName(file)));
            if (ext == ".bin")
                cue.AppendLine(string.Format(CultureInfo.InvariantCulture, "FILE \"{0}\" BINARY", Path.GetFileName(file)));
            else if (ext == ".wav" || ext == ".wave")
                cue.AppendLine(string.Format(CultureInfo.InvariantCulture, "FILE \"{0}\" WAVE", Path.GetFileName(file)));
            else
                return;
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} found.", Path.GetFileName(file)));
            cue.AppendLine(string.Format(CultureInfo.InvariantCulture, "  TRACK {0:D2} AUDIO", track++));
            if (track == 3)
                cue.AppendLine("    PREGAP 00:02:00");
            cue.AppendLine("    INDEX 01 00:00:00");
        }

        private static string ExportCueToFile(StringBuilder cue, string folderPath, string isoFilePath)
        {
            string cueFilePath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(isoFilePath) + ".cue");
            File.WriteAllText(cueFilePath, cue.ToString());
            return cueFilePath;
        }

        private static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Environment.Exit(-1);
        }
    }
}
