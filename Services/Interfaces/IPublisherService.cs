using Library_Management_System.DTOs.Publishers;
using Library_Management_System.Services.Results;

namespace Library_Management_System.Services.Interfaces;

public interface IPublisherService
{
    Task<IReadOnlyList<PublisherResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PublisherResponse?> GetByIdAsync(int publisherId, CancellationToken cancellationToken = default);
    Task<PublisherResponse> CreateAsync(CreatePublisherRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int publisherId, UpdatePublisherRequest request, CancellationToken cancellationToken = default);
    Task<DeleteResult> DeleteAsync(int publisherId, CancellationToken cancellationToken = default);
}
