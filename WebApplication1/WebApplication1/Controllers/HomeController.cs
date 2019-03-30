using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public class areaCls
        {
            public string areaName;
            public int value;
            public int[] mapps;
            public int[] rectps;
        }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 上传本地文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadifyLocalFile()
        {
            Dictionary<string, object> imgUrl = new Dictionary<string, object>();
            List<areaCls> areaList = new List<areaCls>();
            
            try
            {
                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                //没有文件上传，直接返回
                if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
                {
                    return HttpNotFound();
                }
                //获取文件完整文件名(包含绝对路径)
                //文件存放路径格式：/Resource/ResourceFile/{userId}{data}/{guid}.{后缀名}
                string fileGuid = Guid.NewGuid().ToString();
                long filesize = files[0].ContentLength;
                string FileEextension = Path.GetExtension(files[0].FileName);
                string uploadDate = DateTime.Now.ToString("yyyyMMdd");

                string virtualPath = string.Format("~/Resource/zip/" + files[0].FileName.Replace(FileEextension,"/")  +  files[0].FileName);
                string fullFileName = this.Server.MapPath(virtualPath);
                //创建文件夹
                string path = Path.GetDirectoryName(fullFileName);
                Directory.CreateDirectory(path);
                if (!System.IO.File.Exists(fullFileName))
                {
                    //保存文件
                    files[0].SaveAs(fullFileName);
                    
                }

                //解压保存的路径
                string saveDir = fullFileName.Substring(0, fullFileName.LastIndexOf("\\") + 1);
                unzip(fullFileName , saveDir);

                imgUrl =  getPIC(saveDir);
                //解压出来的是文件夹则重新进入文件夹获取Excel
                if (imgUrl.Count == 0)
                {
                    DirectoryInfo TheFolder = new DirectoryInfo(saveDir);
                    foreach (DirectoryInfo NextFile in TheFolder.GetDirectories()) //查找文件夹
                    {
                        imgUrl = getPIC(NextFile.FullName + "\\");
                    }
                }
                return Content(JsonConvert.SerializeObject(imgUrl));
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        /// <summary>
        /// 各区初始化
        /// </summary>
        /// <returns></returns>
        public List<areaCls> areaListInit()
        {
            List<areaCls> list = new List<areaCls>();
            areaCls aCls1 = new areaCls();
            aCls1.areaName = "西北地区";aCls1.mapps = new int[] { 400, 290 }; aCls1.rectps = new int[] {20,330 };
            list.Add(aCls1);
            areaCls aCls2 = new areaCls();
            aCls2.areaName = "华北地区"; aCls2.mapps = new int[] { 570, 248 }; aCls2.rectps = new int[] { 20, 365 };
            list.Add(aCls2);
            areaCls aCls3 = new areaCls();
            aCls3.areaName = "东北地区"; aCls3.mapps = new int[] { 690, 180 }; aCls3.rectps = new int[] { 20, 400 };
            list.Add(aCls3);
            areaCls aCls4 = new areaCls();
            aCls4.areaName = "华中地区"; aCls4.mapps = new int[] { 585, 397 }; aCls4.rectps = new int[] { 20, 435 };
            list.Add(aCls4);
            areaCls aCls5 = new areaCls();
            aCls5.areaName = "华南地区"; aCls5.mapps = new int[] { 610, 475 }; aCls5.rectps = new int[] { 20, 470 };
            list.Add(aCls5);
            areaCls aCls6 = new areaCls();
            aCls6.areaName = "华东地区"; aCls6.mapps = new int[] { 651, 363 }; aCls6.rectps = new int[] { 20, 505 };
            list.Add(aCls6);
            areaCls aCls7 = new areaCls();
            aCls7.areaName = "西南地区"; aCls7.mapps = new int[] { 433, 392 }; aCls7.rectps = new int[] { 20, 540 };
            list.Add(aCls7);
            return list;
        }

        public Dictionary<string, object> getPIC(string saveDir)
        {
            Dictionary<string, object> imgUrl = new Dictionary<string, object>();
            DirectoryInfo TheFolder = new DirectoryInfo(saveDir);
            foreach (FileInfo NextFile in TheFolder.GetFiles()) //查找文件
            {
                if (NextFile.Name.Contains("嘉实多") && NextFile.Name.Contains("地区") && (NextFile.Name.Contains(".xls") || NextFile.Name.Contains(".xlxs")))
                {
                    List<areaCls> alist = GetExcelData(saveDir, NextFile);
                    //绘图
                    string fileName = ReadImageFile(saveDir + NextFile.Name.Replace(".xls", "").Replace(".xlxs", "") + ".png", this.Server.MapPath("~/Resource/map.png"), alist);
                    imgUrl.Add("jsd_area", fileName.Substring(fileName.IndexOf("\\Resource"), fileName.Length - fileName.IndexOf("\\Resource")).ToString());
                }
                if (NextFile.Name.Contains("BP") && NextFile.Name.Contains("地区") && (NextFile.Name.Contains(".xls") || NextFile.Name.Contains(".xlxs")))
                {
                    List<areaCls> alist = GetExcelData(saveDir, NextFile);
                    //绘图
                    string fileName = ReadImageFile(saveDir + NextFile.Name.Replace(".xls","").Replace(".xlxs","") + ".png", this.Server.MapPath("~/Resource/map.png"), alist);
                    imgUrl.Add("bp_area", fileName.Substring(fileName.IndexOf("\\Resource"), fileName.Length - fileName.IndexOf("\\Resource")).ToString() );
                }
                if (NextFile.Name.Contains("嘉实多") && NextFile.Name.Contains("省份") && (NextFile.Name.Contains(".xls") || NextFile.Name.Contains(".xlxs")))
                {
                    //加载 嘉实多 各省份数据
                    imgUrl.Add("jsd_p", GetProvineDat(saveDir, NextFile));
                }
                if (NextFile.Name.Contains("BP") && NextFile.Name.Contains("省份") && (NextFile.Name.Contains(".xls") || NextFile.Name.Contains(".xlxs")))
                {
                    //加载BP各省份数据
                    imgUrl.Add("bp_p", GetProvineDat(saveDir, NextFile));
                }
            }
            return imgUrl;
        }

        /// <summary>
        /// 获取省份数据
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="NextFile"></param>
        /// <returns></returns>
        public List<areaCls> GetProvineDat(string saveDir, FileInfo NextFile)
        {
            List<areaCls> ProvinceDat = new List<areaCls>(); //各省数据
            ISheet sheet = null;
            using (FileStream file = new FileStream(saveDir + NextFile, FileMode.Open, FileAccess.Read))
            {
                string FEextension = Path.GetExtension(NextFile.Name);
                if (FEextension == ".xls")
                {
                    HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
                    sheet = hssfworkbook.GetSheetAt(0);
                }
                else
                {
                    XSSFWorkbook xssfworkbook = new XSSFWorkbook(file);
                    sheet = xssfworkbook.GetSheetAt(0);
                }
                //遍历所有行
                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    //得到i行
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    ////遍历i行的单元格
                    //for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                    //{
                        if (row.LastCellNum > 7 && row.GetCell(0).CellType == CellType.Numeric)
                        {
                            int value = Convert.ToInt32(row.GetCell(0).NumericCellValue);
                            if (value > 0 && value < 32) {
                                areaCls cls = new areaCls();
                                cls.value = Convert.ToInt32(row.GetCell(2).NumericCellValue);
                                cls.areaName = row.GetCell(1).ToString();
                                ProvinceDat.Add(cls);
                                //ProvinceDat.Add(row.GetCell(1).ToString(), Convert.ToInt32(row.GetCell(2).NumericCellValue));
                            }
                        }
                        if (row.LastCellNum > 7 && row.GetCell(5).CellType == CellType.Numeric)
                        {
                            int value = Convert.ToInt32(row.GetCell(5).NumericCellValue);
                            if (value > 0 && value < 32)
                            {
                                areaCls cls = new areaCls();
                                cls.value = Convert.ToInt32(row.GetCell(7).NumericCellValue);
                                cls.areaName = row.GetCell(6).ToString();
                                ProvinceDat.Add(cls);
                                // ProvinceDat.Add(row.GetCell(6).ToString(), Convert.ToInt32(row.GetCell(7).NumericCellValue));
                            }
                        }
                        if (row.LastCellNum == 4 && row.GetCell(0).CellType == CellType.Numeric)
                        {
                            int value = Convert.ToInt32(row.GetCell(0).NumericCellValue);
                            if (value > 0 && value < 32)
                            {
                                areaCls cls = new areaCls();
                                cls.value = Convert.ToInt32(row.GetCell(2).NumericCellValue);
                                cls.areaName = row.GetCell(1).ToString();
                                ProvinceDat.Add(cls);
                            //ProvinceDat.Add(row.GetCell(1).ToString(), Convert.ToInt32(row.GetCell(2).NumericCellValue));
                            }
                        }
                    //}
                }
            }
            return ProvinceDat;
        }


        /// <summary>
        /// 获取excel 区数据
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="NextFile"></param>
        /// <returns></returns>
        public List<areaCls> GetExcelData(string saveDir, FileInfo NextFile)
        {
            string[] area = new string[] { "华北地区", "华东地区", "华南地区", "华中地区", "西南地区", "西北地区", "东北地区" };
            List<areaCls> areaList = areaListInit();
            List<areaCls> alist = new List<areaCls>();
            ISheet sheet = null;
            using (FileStream file = new FileStream(saveDir + NextFile, FileMode.Open, FileAccess.Read))
            {
                string FEextension = Path.GetExtension(NextFile.Name);
                if (FEextension == ".xls")
                {
                    HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
                    sheet = hssfworkbook.GetSheetAt(0);
                }
                else
                {
                    XSSFWorkbook xssfworkbook = new XSSFWorkbook(file);
                    sheet = xssfworkbook.GetSheetAt(0);
                }
                //遍历所有行
                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    //得到i行
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    //遍历i行的单元格
                    for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            if (area.Count(p => p == row.GetCell(j).ToString()) > 0)
                            {
                                int value = Convert.ToInt32(row.GetCell(j + 1).NumericCellValue);
                                foreach (areaCls entity in areaList)
                                {
                                    if (entity.areaName == row.GetCell(j).ToString())
                                    {
                                        entity.value = value;
                                        alist.Add(entity);
                                    }

                                }
                            }
                        }
                    }
                }

            }
            return alist;
        }

    
    

        /// <summary>
        /// 通过FileStream 来打开文件，这样就可以实现不锁定Image文件，到时可以让多用户同时访问Image文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string ReadImageFile(string fileName ,string path , List<areaCls> alist)
        {
            FileStream fs = System.IO.File.OpenRead(path); //OpenRead
            int filelength = 0;
            filelength = (int)fs.Length; //获得文件长度 
            Byte[] image = new Byte[filelength]; //建立一个字节数组 
            fs.Read(image, 0, filelength); //按字节流读取 
            System.Drawing.Image result = System.Drawing.Image.FromStream(fs);
            fs.Close();
            Bitmap bit = new Bitmap(result);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bit);
            //字体大小
            float fontSize = 15.0f;
            //定义字体
            System.Drawing.Font font = new System.Drawing.Font("微软雅黑", fontSize, System.Drawing.FontStyle.Regular);
            //红笔刷，画文字用
            Brush redBrush = new SolidBrush(System.Drawing.Color.Red);
            foreach (areaCls cls in alist)
            {
                if (cls.value != 0)
                {
                    RectangleF textArea = new RectangleF(cls.mapps[0] - 15, cls.mapps[1] - 7, 100, 40);
                    g.DrawString("(" + cls.value.ToString() + ")", font, redBrush, textArea);
                    RectangleF textArea2 = new RectangleF(cls.rectps[0] + 92, cls.rectps[1] - 2, 100, 40);
                    g.DrawString("(" + cls.value.ToString() + ")", font, redBrush, textArea2);
                }
            }
            bit.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
            return fileName;
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="saveDir"></param>
        public void unzip(string fullName , string saveDir)
        {
            if (string.IsNullOrEmpty(ExistsWinRar())) {
                return;
            }
            DeCompressRar(fullName, saveDir);
        }

        /// <summary>
        /// 判断是否有压缩软件
        /// </summary>
        /// <returns></returns>
        public string ExistsWinRar()
        {
            string result = string.Empty;

            string key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe";
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(key);
            if (registryKey != null)
            {
                result = registryKey.GetValue("").ToString();
            }
            registryKey.Close();

            return result;
        }

        /// <summary>
        /// 将格式为rar的压缩文件解压到指定的目录
        /// </summary>
        /// <param name="rarFileName">要解压rar文件的路径</param>
        /// <param name="saveDir">解压后要保存到的目录</param>
        public void DeCompressRar(string rarFileName, string saveDir)
        {
            string regKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe";
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(regKey);
            string winrarPath = registryKey.GetValue("").ToString();
            registryKey.Close();
            string winrarDir = System.IO.Path.GetDirectoryName(winrarPath);
            String commandOptions = string.Format("x {0} {1} -y", rarFileName, saveDir);

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = System.IO.Path.Combine(winrarDir, "rar.exe");
            processStartInfo.Arguments = commandOptions;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            Process process = new Process();
            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();
            process.Close();
        }
    }
}