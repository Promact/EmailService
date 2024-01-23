namespace EmailService
{
    public class AttachmentData
    {
        public AttachmentData(byte[] content, string fileName, string contentType)
        {
            Content = content;
            ContentType = contentType;
            FileName = fileName;
        }
        public byte[] Content { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}