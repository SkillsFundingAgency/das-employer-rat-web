namespace SFA.DAS.EmployerRequestApprenticeTraining.Infrastructure.Api.Responses
{
    public enum RequestType : byte
    {
        Shortlist = 0,
        CourseDetail = 1,
        Providers = 2
    }

    public enum Status : byte
    {
        Active = 0
    }
}
