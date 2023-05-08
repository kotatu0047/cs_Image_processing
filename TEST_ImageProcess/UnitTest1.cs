using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageProcess;
using System.Drawing.Imaging;

namespace TEST_ImageProcess
{
    [TestClass]
    public class UnitTest1
    {
        private static string _testImageFileName = "test_image.jpg";
        private Bitmap _srcBitmap = null;

        /// <summary>
        /// テスト実行前の準備を行う
        /// </summary>
        /// <exception cref="DirectoryNotFoundException">テストに使うファイルが存在しません</exception>
        public UnitTest1()
        {
            var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
            var testProjectRootPath = currentDir.Parent.Parent.FullName;
            var filePath = Path.Combine(testProjectRootPath, _testImageFileName);

            if (File.Exists(filePath) == false) throw new DirectoryNotFoundException($"テストを実行できません。テストファイル: {filePath} が見つかりません。");
            this._srcBitmap = new Bitmap(filePath);
            Console.WriteLine($"{filePath}を読み込みました");
        }


        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public unsafe void TestMethod1()
        {
        }
    }
}
