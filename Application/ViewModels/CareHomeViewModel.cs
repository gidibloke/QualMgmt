using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Application.ViewModels
{
    public class CareHomeViewModel
    {
        public int Id { get; set; }

        [Display(Name ="Home name")]
        public string HomeName { get; set; }

        [Display(Name = "Post code")]

        public string PostCode { get; set; }

        public DateTime? DateCreated { get; set; }
    }
    //Though the HttpClient class implement IDisposable, declaring and instantiating it within a using statement is not preferred because when the HttpClient object gets disposed of, the underlying socket is not immediately released, which can lead to a socket exhaustion problem.
    public class CareViewModelValidator : AbstractValidator<CareHomeViewModel>
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CareViewModelValidator(IHttpClientFactory httpClientFactory)
        {
            RuleFor(x => x.HomeName).NotEmpty().WithMessage("Please provide home name");
            RuleFor(n => n.PostCode).Must(PostCodeValidator).When(a => a.PostCode != null).WithMessage("{PropertyValue} is not a valid Postcode");
            _httpClientFactory = httpClientFactory;
        }

        public bool PostCodeValidator(string nnum)
        {
            Task<bool> task = Task.Run(async () => await RemoteValidator(nnum));
            return task.Result;
        }

        public async Task<bool> RemoteValidator(string tovalidate)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                string validateurl = $"https://api.postcodes.io/postcodes/{tovalidate}/validate";

                HttpResponseMessage res = await client.GetAsync(validateurl);

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    string responsecontent = await res.Content.ReadAsStringAsync();
                    var parsed = JsonNode.Parse(responsecontent).AsObject();

                    bool valid = (bool)parsed["result"];

                    if (valid)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    // validator api fauty assume OK ?
                    return true;
                }
            }
            catch
            {
                return true;
            }
        }
    }
}
