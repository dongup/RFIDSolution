using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.Server.Utils
{
    public class ExcelHelper
    {
        public XSSFWorkbook hssfworkbook;

        private string filePath;

        public ExcelHelper()
        {

        }

        public ExcelHelper(string templatePath)
        {
            InitializeWorkbook(templatePath);
            filePath = templatePath;
        }

        private void InitializeWorkbook(string path)
        {
            //read the template via FileStream, it is suggested to use FileAccess.Read to prevent file lock.
            //book1.xls is an Excel-2007-generated file, so some new unknown BIFF records are added. 
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);

            hssfworkbook = new XSSFWorkbook(file);
        }

        public void WriteToFile(string path)
        {
            //Write the stream data of workbook to the root directory
            FileStream file = new FileStream(path, FileMode.Create);
            hssfworkbook.Write(file);
            file.Close();
        }

        public void SaveChange()
        {
            //Write the stream data of workbook to the root directory
            FileStream file = new FileStream(filePath, FileMode.Create);
            hssfworkbook.Write(file);
            file.Close();
        }

    }
}
