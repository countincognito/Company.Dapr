namespace Company.iFX.Test
{
    public static class ServiceMock 
    {
       public static Func<T,Task> Create<T>(Func<T,Task> creationFunc) where T : class
       {
            return creationFunc;
       }
    }
}
