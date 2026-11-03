namespace SecurityFootage.Domain
{
  public enum RequestResult
        {
            InvalidUser,
            InvalidAccess,
            Success,
            Conflict,
            FootageNotFound,
            SasGenerationFailed
        }
}
