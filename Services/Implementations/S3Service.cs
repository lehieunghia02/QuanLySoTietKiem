namespace QuanLySoTietKiem.Services.Implementations;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
public class S3Service
{
   private readonly IAmazonS3 _s3;
   private readonly string _bucket;

   public S3Service(IConfiguration config)
   {
      _bucket = config["AWS:BucketName"];

      _s3 = new AmazonS3Client(
         config["AWS:AccessKey"],
         config["AWS:SecretKey"],
         RegionEndpoint.GetBySystemName(config["AWS:Region"])
      );
   }

      public async Task<string> UploadAsync(IFormFile file, string folder)
      {
         var fileName = $"{folder}/{Guid.NewGuid()}_{file.FileName}";
         
         using var stream = file.OpenReadStream();
         var transfer = new  TransferUtility(_s3);
         await transfer.UploadAsync(stream, _bucket, fileName);
         return $"https://{_bucket}.s3.amazonaws.com/{fileName}";
      }
      public async Task<string> UploadAvatarAsync(IFormFile file, string userId)
      {
         if (file == null || file.Length == 0)
            throw new ArgumentException("Avatar file is required.");

         if (!file.ContentType.StartsWith("image/"))
            throw new ArgumentException("Only image files are allowed.");

         if (file.Length > 5 * 1024 * 1024)
            throw new ArgumentException("Avatar size must be less than 5MB.");

         var extension = Path.GetExtension(file.FileName);
         var fileName = $"{Guid.NewGuid()}{extension}";
         var key = $"avatars/{userId}/{fileName}";

         using var stream = file.OpenReadStream();
         var transferUtility = new TransferUtility(_s3);

         await transferUtility.UploadAsync(stream, _bucket, key);

         return $"https://{_bucket}.s3.amazonaws.com/{key}";
      }
   
   public async Task DeleteAsync(string fileUrl)
   {
      if (string.IsNullOrWhiteSpace(fileUrl) || !fileUrl.Contains(".amazonaws.com/"))
         return;

      var key = fileUrl.Split(".amazonaws.com/")[1];
      await _s3.DeleteObjectAsync(_bucket, key);
   }
}