using EGXMonitoring.Shared;
using EGXMonitoring.Shared.DTOS;
using Microsoft.EntityFrameworkCore;

namespace EGXMonitoring.Server.Services.MailService
{
   
    public class MailService : IMailService
    {
        private readonly DataContext _context;
        public MailService(DataContext context)
        {
            _context = context;
        }
        public async Task<ServiceResponse<Mail>> AddOrEdit(Mail mail )
        {
            try
            {
                var result = await _context.Mails.ToListAsync();
                if(result.Count() ==  0 || result == null)
                {
                    await _context.Mails.AddAsync(mail);
                    await _context.SaveChangesAsync();  
                }
                else
                {
                     _context.Mails.Update(mail);
                    await _context.SaveChangesAsync();
                }
                return new ServiceResponse<Mail>()
                {
                    Data = mail,
                    Success = true,
                    Message = "Added or updated successfully "
                };
            }
            catch(Exception ex)
            {
                return new ServiceResponse<Mail>()
                {
                    Data = null,
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResponse<Mail>> GetMail()
        {
            try
            {
                var result = await _context.Mails.FirstOrDefaultAsync();
                if (result != null)
                {
                    return new ServiceResponse<Mail>()
                    {
                        Data = result,
                        Success = true,
                        Message = "Mail Retrived "
                    };
                }
                else
                {
                    return new ServiceResponse<Mail>()
                    {
                        Data = null,
                        Success = false,
                        Message = "No Mail Template Found"
                    };
                }
                
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Mail>()
                {
                    Data = null,
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
