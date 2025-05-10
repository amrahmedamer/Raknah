namespace Raknah.Services
{
    public interface IReservationServices
    {
        public Task<Result> CancelReservationAsync(int reservationId);
        public Task CancelReservationIfNotArrivedAsync(int reservationId);
        public Task<Result<ReservationResponse>> CreateReservationAsync(ReservationRequest requeststring, string userId);
        Task<Result> OpenGateAsync(int reservationId);

        public Task<Result<PendingAndCanceledReservationResponse>> GetPendingReservations(string userId);
        public Task<Result<ActiveReservationResponse>> GetActiveReservations(string userId);
        public Task<Result<List<PendingAndActiveReservationResponse>>> GetPendingOrActiveReservations(string userId);

        public Task<Result<List<CompletedAndCanceledReservationResponse>>> GetCompletedReservations(string userId);
        public Task<Result<List<PendingAndCanceledReservationResponse>>> GetCanceledReservations(string userId);
        public Task<Result<List<CompletedAndCanceledReservationResponse>>> GetCompletedAndCanceledReservations(string userId);







    }
}
