using System.Collections.Generic;
using consumer.Entities;
using consumer.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace consumer.Controllers
{
    [Route("api/[controller]")]
    public class EmailsController
    {
        private readonly IEmailRepository _emailRepository;

        public EmailsController(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        [HttpGet]
        public List<Email> GetAll()
        {
            return _emailRepository.GetAll();
        }
    }
}