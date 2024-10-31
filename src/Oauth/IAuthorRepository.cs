public interface IAuthorRepository
{
    public Task<Author> GetAuthorByName(string author);
    public Task<Author> GetAuthorByEmail(string email);
    public Task<int> GetHighestAuthorId();
    public Task WriteAuthor(Author author);
    public static List<AuthorDTO> WrapInDTO(List<Author> authors);
}