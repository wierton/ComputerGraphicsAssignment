using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace ComputerGraphicsWork
{
    public class UserLog
    {
        private String fileName;
        private String oldString;
        private int count = 1;
        public UserLog(String inFileName)
        {
            fileName = inFileName;
        }

        ~UserLog()
        {
            this.write("~~~");
        }
        public static String getCallerInfo()
        {
            StackTrace trace = new StackTrace();
            StackFrame frame = trace.GetFrame(3);//1代表上级，2代表上上级，以此类推  
            MethodBase method = frame.GetMethod();
            String className = method.ReflectedType.Name;
            return String.Format("{0},{1}:{2}:", className, method.Name, frame.GetFileLineNumber());
        }

        public void writeToFile(String s)
        {
            /*
            FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate | FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(String.Format("[{0}]:\t{2}\n", count, getCallerInfo(), s));
            sw.Flush();
            sw.Close();
            fs.Close();
            */
        }

        public void write(String s)
        {
            if (oldString == s)
            {
                count++;
            }
            else
            {
                writeToFile(oldString);
                count = 1;
                oldString = s;
            }
        }
    }

}
