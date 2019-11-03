--select ContactEmail, CharIndex('@', ContactEmail), Left(ContactEmail, CharIndex('@', ContactEmail)) + 'petesoft.com' from mcJobs where ContactEmail <> ''
--update mcJobs set ContactEmail = Left(ContactEmail, CharIndex('@', ContactEmail)) + 'petesoft.com' where ContactEmail <> ''


select Email, CharIndex('@', Email), Left(Email, CharIndex('@', Email)) + 'petesoft.com' from Contacts where Email <> ''
--update Contacts set Email = Left(Email, CharIndex('@', Email)) + 'petesoft.com' where Email <> ''



