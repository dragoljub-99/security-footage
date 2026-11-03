namespace SecurityFootage.Domain
{
    
    public enum AuditOutcome
    {
        SUCCEEDED,
        USERNOTFOUND,
        ACCESSDENIED,
        CONFLICT,
        LOCATIONNOTFOUND,
        FOOTAGENOTFOUND,
        SASGENERATIONFAILED
    }
}