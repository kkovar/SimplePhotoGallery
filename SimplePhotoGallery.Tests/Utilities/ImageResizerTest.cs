using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplePhotoGallery.Utilities;
using System.Drawing;
using System.Drawing.Imaging;

namespace SimplePhotoGallery.Tests.Utilities
{
    [TestClass]
    public class ImageResizerTest
    {
        //[TestMethod]
        public void TestMethod1()
        {
            //set a working directory
            string WorkingDirectory = @".\";

            // note that this worked in the sample project but the file needs to be copied
            //create a image object containing a vertical photograph
            Image imgPhotoVert = Image.FromFile(WorkingDirectory + @"\imageresize_vert.jpg");
            Image imgPhotoHoriz = Image.FromFile(WorkingDirectory + @"\imageresize_horiz.jpg");
            Image imgPhoto = null;

            imgPhoto = ImageResize.ScaleByPercent(imgPhotoVert, 50);
            imgPhoto.Save(WorkingDirectory + @"\images\imageresize_1.jpg", ImageFormat.Jpeg);
            imgPhoto.Dispose();

            imgPhoto = ImageResize.ConstrainProportions(imgPhotoVert, 200, ImageResize.Dimensions.Width);
            imgPhoto.Save(WorkingDirectory + @"\images\imageresize_2.jpg", ImageFormat.Jpeg);
            imgPhoto.Dispose();

            imgPhoto = ImageResize.FixedSize(imgPhotoVert, 200, 200);
            imgPhoto.Save(WorkingDirectory + @"\images\imageresize_3.jpg", ImageFormat.Jpeg);
            imgPhoto.Dispose();

            imgPhoto = ImageResize.Crop(imgPhotoVert, 200, 200, ImageResize.AnchorPosition.Center);
            imgPhoto.Save(WorkingDirectory + @"\images\imageresize_4.jpg", ImageFormat.Jpeg);
            imgPhoto.Dispose();

            imgPhoto = ImageResize.Crop(imgPhotoHoriz, 200, 200, ImageResize.AnchorPosition.Center);
            imgPhoto.Save(WorkingDirectory + @"\images\imageresize_5.jpg", ImageFormat.Jpeg);
            imgPhoto.Dispose();


        }
    }
}
