namespace Domain.Entities
{
    public class EntityBase
    {
        public int Id { get; set; }
        public DateTime DataCriacao { get; set; }
        public EntityBase()
        {
            DataCriacao = DateTime.Now;
        }
    }
}
