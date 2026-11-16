using BookingService.Application.Abstractions.Documents;
using BookingService.Application.Abstractions.Services;
using BookingService.Application.DTOs.Analytics;
using BookingService.Application.DTOs.Requests;
using BookingService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Api.Controllers
{
    /// <summary>
    /// Controller responsible for managing booking transactions, seat reservations, PDF tickets, and analytics.
    /// </summary>
    [Route("api/booking")]
    [ApiController]
    public class BookingController(
        IBookingService bookingService,
        IBookingExcelService bookingExcelService,
        ITicketPdfService ticketPdfService,
        ILogger<BookingController> logger) : ControllerBase
    {
        #region Configuration Fields
        private readonly IBookingService _bookingService = bookingService;
        private readonly IBookingExcelService _bookingExcelService = bookingExcelService;
        private readonly ITicketPdfService _ticketPdfService = ticketPdfService;
        private readonly ILogger<BookingController> _logger = logger;
        #endregion

        #region Get Bookings
        /// <summary>
        /// Retrieves a list of all system bookings (Admin only).
        /// </summary>
        /// <response code="200">Returns the complete list of bookings.</response>
        /// <response code="401">Unauthorized if authentication token is missing or invalid.</response>
        /// <response code="403">Forbidden if the user lacks Admin privileges.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<Booking>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBookings()
        {
            try
            {
                _logger.LogInformation("GetBookings endpoint called.");
                var bookings = await _bookingService.GetBookingAsync();
                _logger.LogInformation("Successfully retrieved {RecordCount} bookings.", bookings.Count());
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all bookings.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving bookings.", details = ex.Message });
            }
        }
        #endregion

        #region Get Booking By ID
        /// <summary>
        /// Retrieves a specific booking record by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the booking.</param>
        /// <response code="200">Returns the matching booking entity.</response>
        /// <response code="404">Returned if the booking does not exist.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Booking), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBookingById(int id)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(id);
                if (booking == null)
                {
                    _logger.LogWarning("Booking with ID {BookingId} not found.", id);
                    return NotFound(new { message = $"Booking with ID {id} was not found." });
                }
                return Ok(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking with ID {BookingId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while fetching the booking.", details = ex.Message });
            }
        }
        #endregion

        #region Get Bookings By User ID
        /// <summary>
        /// Retrieves all booking records associated with a specific user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <response code="200">Returns the list of bookings for the user.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("user/{userId:int}")]
        [ProducesResponseType(typeof(IEnumerable<Booking>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBookingsByUserId(int userId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingByUserIdAsync(userId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings for user ID {UserId}.", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving user bookings.", details = ex.Message });
            }
        }
        #endregion

        #region Get Bookings By Show ID
        /// <summary>
        /// Retrieves all bookings made for a specific movie show.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <response code="200">Returns the list of bookings for the show.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("show/{showId:int}")]
        [ProducesResponseType(typeof(IEnumerable<Booking>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBookingsByShow(int showId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingByShowIdAsync(showId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings for show ID {ShowId}.", showId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving show bookings.", details = ex.Message });
            }
        }
        #endregion

        #region Get Booked Seat Count By Show ID
        /// <summary>
        /// Gets the total count of seats booked for a given show.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <response code="200">Returns the total booked seat count.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("show/{showId:int}/seatcount")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBookedSeatCount(int showId)
        {
            try
            {
                var count = await _bookingService.GetBookedSeatCountByShowAsync(showId);
                return Ok(new { showId, bookedSeatCount = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching booked seat count for show ID {ShowId}.", showId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while getting seat counts.", details = ex.Message });
            }
        }
        #endregion

        #region Get Booked Seats By Booking ID
        /// <summary>
        /// Retrieves the specific seat numbers assigned to a booking.
        /// </summary>
        /// <param name="bookingId">The booking identifier.</param>
        /// <response code="200">Returns a list of seat numbers.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{bookingId:int}/seats")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBookedSeats(int bookingId)
        {
            try
            {
                var seats = await _bookingService.GetBookedSeatsAsync(bookingId);
                return Ok(seats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving seats for booking ID {BookingId}.", bookingId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving booked seats.", details = ex.Message });
            }
        }
        #endregion

        #region Movie Wise Collection Analytics
        /// <summary>
        /// Retrieves aggregated box-office collection revenue grouped by movie title.
        /// </summary>
        /// <response code="200">Returns the movie collection revenue list.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("movie-collection")]
        [ProducesResponseType(typeof(List<MovieCollectionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMovieWiseCollection()
        {
            try
            {
                var collection = await _bookingService.GetMovieWiseCollectionAsync();
                return Ok(collection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating movie-wise collection.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while calculating collections.", details = ex.Message });
            }
        }
        #endregion

        #region Create Booking
        /// <summary>
        /// Creates a new basic booking record.
        /// </summary>
        /// <param name="booking">The booking entity payload.</param>
        /// <response code="201">Returns the newly created booking record.</response>
        /// <response code="400">Returned if request validation fails.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Booking), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
        {
            try
            {
                if (booking == null)
                    return BadRequest(new { message = "Booking payload cannot be null." });

                _logger.LogInformation("Creating booking record.");
                var createdBooking = await _bookingService.CreateBookingAsync(booking);
                return CreatedAtAction(nameof(GetBookingById), new { id = createdBooking.BookingId }, createdBooking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the booking.", details = ex.Message });
            }
        }
        #endregion

        #region Create Booking With Seats (Transactional)
        /// <summary>
        /// Creates a complete booking along with seat reservations and publishes a confirmation event.
        /// </summary>
        /// <param name="request">The booking request containing show, user, and seat details.</param>
        /// <response code="201">Returns the created booking identifier.</response>
        /// <response code="400">Returned if seat conflict occurs or request is invalid.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("create-with-seats")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBookingWithSeats([FromBody] CreateBookingWithSeatsRequest request)
        {
            try
            {
                var bookingId = await _bookingService.CreateBookingWithSeatsAsync(request);

                return StatusCode(StatusCodes.Status201Created, new
                {
                    message = "Booking created successfully with seats.",
                    bookingId
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Seat reservation conflict during booking creation.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking with seats.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during booking creation.", details = ex.Message });
            }
        }
        #endregion

        #region Bulk Book Seats (TVP)
        /// <summary>
        /// Bulk books and locks seats for an existing booking using Table-Valued Parameters.
        /// </summary>
        /// <param name="request">The bulk seat booking request object.</param>
        /// <response code="200">Seats successfully booked.</response>
        /// <response code="400">Returned if seats are already locked or booked.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("bulk-seats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> BulkBookSeats([FromBody] BulkSeatBookingRequest request)
        {
            try
            {
                await _bookingService.BulkBookSeatsAsync(
                    request.BookingId,
                    request.ShowId,
                    request.SeatNos
                );

                return Ok(new { message = "Seats booked successfully using TVP." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Bulk seat locking conflict.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk seat booking.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during bulk seat reservation.", details = ex.Message });
            }
        }
        #endregion

        #region Upload Excel Seats
        /// <summary>
        /// Uploads an Excel spreadsheet to bulk-import and reserve seat allocations.
        /// </summary>
        /// <param name="file">The Excel file stream (.xlsx).</param>
        /// <response code="200">Seats successfully booked from the file.</response>
        /// <response code="400">Returned if file is missing, empty, or format is invalid.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("upload-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded or file is empty." });
            }

            try
            {
                using var stream = file.OpenReadStream();
                await _bookingExcelService.UploadAndBulkInsertSeatsAsync(stream);
                return Ok(new { message = "Seats booked successfully from Excel file." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while processing uploaded Excel file.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Excel upload.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while parsing the Excel file.", details = ex.Message });
            }
        }
        #endregion

        #region Cancel Booking
        /// <summary>
        /// Cancels an existing booking by updating its status.
        /// </summary>
        /// <param name="id">The booking identifier.</param>
        /// <response code="200">Returns true if cancellation succeeded.</response>
        /// <response code="404">Returned if the booking was not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("cancel/{id:int}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelBooking(int id)
        {
            try
            {
                var success = await _bookingService.CancelBookingAsync(id);
                if (!success)
                {
                    return NotFound(new { message = $"Booking with ID {id} not found." });
                }
                return Ok(new { success, message = "Booking cancelled successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking ID {BookingId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while cancelling the booking.", details = ex.Message });
            }
        }
        #endregion

        #region Update Booking Status
        /// <summary>
        /// Updates the payment or operational status of a booking.
        /// </summary>
        /// <param name="id">The booking identifier.</param>
        /// <param name="paymentStatus">The new status string (e.g., Completed, Cancelled, Pending).</param>
        /// <response code="200">Returns true if update succeeded.</response>
        /// <response code="404">Returned if the booking was not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("status/{id:int}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromQuery] string paymentStatus)
        {
            try
            {
                var result = await _bookingService.UpdateBookingStatusAsync(id, paymentStatus);
                if (!result)
                {
                    return NotFound(new { message = $"Booking with ID {id} not found." });
                }
                return Ok(new { success = result, message = "Booking status updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for booking ID {BookingId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating status.", details = ex.Message });
            }
        }
        #endregion

        #region Download Ticket PDF
        /// <summary>
        /// Generates and downloads the PDF ticket document for a specific booking.
        /// </summary>
        /// <param name="bookingId">The booking identifier.</param>
        /// <response code="200">Returns the PDF file binary stream.</response>
        /// <response code="404">Returned if booking or related assets were not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("download/{bookingId:int}")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadTicket(int bookingId)
        {
            try
            {
                var ticket = await _ticketPdfService.BuildTicketPdfModelAsync(bookingId);
                var pdfBytes = _ticketPdfService.GenerateTicketPdf(ticket);

                return File(
                    pdfBytes,
                    "application/pdf",
                    $"Ticket_{bookingId}.pdf"
                );
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Booking asset not found during PDF download.");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating ticket PDF for booking ID {BookingId}.", bookingId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while generating the PDF ticket.", details = ex.Message });
            }
        }
        #endregion

        #region Get QR Code Image
        /// <summary>
        /// Generates and returns a PNG QR code image for a booking ticket.
        /// </summary>
        /// <param name="bookingId">The booking identifier.</param>
        /// <response code="200">Returns the PNG image stream.</response>
        /// <response code="404">Returned if booking was not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("qr/{bookingId:int}")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetQrCode(int bookingId)
        {
            try
            {
                var ticket = await _ticketPdfService.BuildTicketPdfModelAsync(bookingId);

                return File(
                    ticket.QrCodeImage,
                    "image/png",
                    $"QR_{bookingId}.png"
                );
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Booking not found for QR code generation.");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code for booking ID {BookingId}.", bookingId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while generating the QR code.", details = ex.Message });
            }
        }
        #endregion
    }
}