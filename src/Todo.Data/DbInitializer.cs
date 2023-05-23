using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Todo.Data.Models;

namespace Todo.Data;

public class DbInitializer
{
    public void Seed(IServiceProvider serviceProvider)
    {
        Randomizer.Seed = new Random(1337);
        using var context = serviceProvider.GetRequiredService<TodoContext>();
        var itemFaker = new Faker<TodoItem>()
            .RuleFor(t => t.Id, f => Guid.NewGuid())
            .RuleFor(t => t.Completed, f => f.Random.Bool() ? f.Date.Past() : null)
            .RuleFor(t => t.Created, (f, t) => f.Date.Past(refDate: t.Completed))
            .RuleFor(t => t.Text, f => f.Lorem.Sentence());

        var items = itemFaker.Generate(5);

        context.TodoItems.AddRange(items);
        context.SaveChanges();
    }
}