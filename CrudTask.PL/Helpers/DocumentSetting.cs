namespace CrudTask.PL.Helpers
{
    public class DocumentSetting
    {
        public static string UploadFile(IFormFile formFile, string folderName)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(formFile.FileName)}";

            string filePath = Path.Combine(folderPath, fileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);

            formFile.CopyTo(fileStream);

            return fileName;
        }


        public static void DeleteFile(string fileName, string FolderName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\files", FolderName, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

        }
    }
}
