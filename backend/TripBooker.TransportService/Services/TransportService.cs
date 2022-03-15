using System.Transactions;
using TripBooker.TransportService.Model;
using TripBooker.TransportService.Model.Extensions;
using TripBooker.TransportService.Repositories;

namespace TripBooker.TransportService.Services;

internal interface ITransportService
{
    Task AddNewTransport(Transport transport, CancellationToken cancellationToken);
}

internal class TransportService : ITransportService
{
    private readonly ITransportCommandRepository _commandRepository;
    private readonly ITransportViewUpdateRepository _viewRepository;
    private readonly ITransportOptionRepository _optionRepository;

    public TransportService(
        ITransportCommandRepository commandRepository, 
        ITransportViewUpdateRepository viewRepository, 
        ITransportOptionRepository optionRepository)
    {
        _commandRepository = commandRepository;
        _viewRepository = viewRepository;
        _optionRepository = optionRepository;
    }

    public async Task AddNewTransport(Transport transport, CancellationToken cancellationToken)
    {
        var transportOption = await _optionRepository.GetById(transport.OptionId);
        if (transportOption == null)
        {
            throw new ArgumentException($"Option id for transport does not exist (optionId={transport.OptionId}",
                nameof(transport));
        }

        var newView = transport.MapToTransportView(transportOption);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _commandRepository.AddAsync(transport, cancellationToken);
        await _viewRepository.AddAsync(newView, cancellationToken);

        transaction.Complete();
    }
}