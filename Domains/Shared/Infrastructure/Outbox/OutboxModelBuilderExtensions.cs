using Microsoft.EntityFrameworkCore;

namespace CleanDDDArchitecture.Domains.Shared.Infrastructure.Outbox;

public static class OutboxModelBuilderExtensions
{
    /// <summary>Adds the <c>outbox_messages</c> table to a context that raises integration events.</summary>
    public static ModelBuilder ApplyOutbox(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<OutboxMessage>(
            outbox =>
            {
                outbox.ToTable("outbox_messages");
                outbox.HasKey(message => message.Id);
                outbox.Property(message => message.Id).ValueGeneratedNever();
                outbox.Property(message => message.Type).HasMaxLength(256).IsRequired();
                outbox.Property(message => message.Payload).IsRequired();
                outbox.Property(message => message.LastError).HasMaxLength(2000);

                // The dispatcher's query: unprocessed messages, oldest first.
                outbox.HasIndex(message => new { message.ProcessedAtUtc, message.OccurredAtUtc });
            });

        return modelBuilder;
    }
}
