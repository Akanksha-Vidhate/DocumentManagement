using DocumentManagementApi.DataAccess;
using DocumentManagementApi.Models;
using DocumentManagementApi.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace DocumentManagementApi.Controllers
{
    
    [EnableCors(origins: "http://localhost:62714", headers: "*", methods: "*")]
    public class DocumentController : ApiController
    {
        private DocumentDbContext documentDbContent=null;
        public DocumentController()
        {
            documentDbContent = new DocumentDbContext();
        }
        // GET api/<controller>
        public IEnumerable<Document> Get()
        {
            return documentDbContent.Documents.ToList();
        }

        // POST api/<controller>
        public Task<HttpResponseMessage> Post()
        {
            List<string> savedFilePath = new List<string>();
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            string rootPath = HttpContext.Current.Server.MapPath("~/Uploads");
            var provider = new MultipartFileStreamProvider(rootPath);
            var task = Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<HttpResponseMessage>(t =>
                {
                    if (t.IsCanceled || t.IsFaulted)
                    {
                        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                    }
                    foreach (MultipartFileData item in provider.FileData)
                    {
                        try
                        {
                            string name = item.Headers.ContentDisposition.FileName.Replace("\"", "");
                            string newFileName = Guid.NewGuid() + Path.GetExtension(name);
                            File.Move(item.LocalFileName, Path.Combine(rootPath, newFileName));

                            Uri baseuri = new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, string.Empty));
                            string fileRelativePath = "~/Uploads/" + newFileName;
                            Uri fileFullPath = new Uri(baseuri, VirtualPathUtility.ToAbsolute(fileRelativePath));
                            savedFilePath.Add(fileFullPath.ToString());

                            Document doc = new Document();
                            doc.Name = name;
                            doc.DocumentFileName = newFileName;
                            documentDbContent.Documents.Add(doc);
                            documentDbContent.SaveChanges();

                        }
                        catch (Exception ex)
                        {
                            string message = ex.Message;
                        }
                    }                                     
                    return Request.CreateResponse(HttpStatusCode.Created, savedFilePath);
                });

            return task;
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
            var doc= documentDbContent.Documents.FirstOrDefault(x => x.Id == id);
            if(doc!= null)
            {
                documentDbContent.Documents.Remove(doc);
                documentDbContent.SaveChanges();
            }            
        }
    }
}