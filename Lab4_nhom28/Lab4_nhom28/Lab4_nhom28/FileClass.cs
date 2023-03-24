using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4_nhom28
{
    class FileClass
    {
        public static void WriteFile(Dictionary<string, string> dictionary, string file)
        {
            using (FileStream fs = File.OpenWrite(file))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                // Put count.
                writer.Write(dictionary.Count);
                // Write pairs.
                foreach (var pair in dictionary)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value);
                }
            }
        }

        public static Dictionary<string, string> ReadFile(string file)
        {
            var result = new Dictionary<string, string>();
            using (FileStream fs = File.OpenRead(file))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // Get count.
                int count = reader.ReadInt32();
                // Read in all pairs.
                for (int i = 0; i < count; i++)
                {
                    string key = reader.ReadString();
                    string value = reader.ReadString();
                    result[key] = value;
                }
            }
            return result;
        }

    }
}
