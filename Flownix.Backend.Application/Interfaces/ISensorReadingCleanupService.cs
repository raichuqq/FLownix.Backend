using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flownix.Backend.Application.Interfaces
{
    public interface ISensorReadingCleanupService
    {
        Task CleanupOldReadingsAsync(TimeSpan maxAge, CancellationToken cancellationToken = default);
    }
}