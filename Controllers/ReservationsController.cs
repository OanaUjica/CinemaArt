﻿using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Lab1_.NET.Models;
using Lab1_.NET.ViewModels.Reservations;
using Microsoft.AspNetCore.Http;
using System;
using Lab1_.NET.Services;
using System.Collections.Generic;

namespace Lab1_.NET.Controllers
{
    [Authorize(AuthenticationSchemes = "Identity.Application,Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationsService _reservationsService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationsController(IReservationsService reservationsService, UserManager<ApplicationUser> userManager)
        {
            _reservationsService = reservationsService;
            _userManager = userManager;
        }

        /// <summary>
        /// Add a new reservation
        /// </summary>
        /// <response code="201">Add a new reservation</response>
        /// <response code="400">Unable to add the reservation due to validation error</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult> PlaceReservation(NewReservationRequest newReservationRequest)
        {
            var user = new ApplicationUser();
            try
            {
                user = await _userManager?.FindByNameAsync(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }
            catch (ArgumentNullException)
            {
                return Unauthorized("Please login!");
            }

            var reservationServiceResult = await _reservationsService.PlaceReservation(newReservationRequest, user);
            if (reservationServiceResult.ResponseError != null)
            {
                return BadRequest(reservationServiceResult.ResponseError);
            }

            var reservation = reservationServiceResult.ResponseOk;

            return CreatedAtAction("GetReservations", new { id = reservation.Id }, "New reservation successfully added");
        }


        /// <summary>
        /// Get reservations
        /// </summary>
        /// <response code="200">Get reservations</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationsForUserResponse>>> GetAllReservations()
        {
            var user = new ApplicationUser();
            try
            {
                user = await _userManager?.FindByNameAsync(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }
            catch (ArgumentNullException)
            {
                return Unauthorized("Please login!");
            }

            var reservationServiceResult = await _reservationsService.GetAllReservations(user);

            return Ok(reservationServiceResult.ResponseOk);
        }

        /// <summary>
        /// Amend a reservation
        /// </summary>
        /// <response code="204">Amend a reservation</response>
        /// <response code="400">Unable to amend the reservation due to validation error</response>
        /// <response code="404">Reservation not found</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, NewReservationRequest updateReservationRequest)
        {
            var user = new ApplicationUser();
            try
            {
                user = await _userManager?.FindByNameAsync(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }
            catch (ArgumentNullException)
            {
                return Unauthorized("Please login!");
            }

            if (!_reservationsService.ReservationExists(id))
            {
                return NotFound();
            }

            var reservationServiceResult = await _reservationsService.UpdateReservation(id, updateReservationRequest, user);
            if (reservationServiceResult.ResponseError != null)
            {
                return BadRequest(reservationServiceResult.ResponseError);
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a reservation by id
        /// </summary>
        /// <response code="204">Delete a reservation</response>
        /// <response code="404">Reservation not found</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            try
            {
                var user = await _userManager?.FindByNameAsync(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }
            catch (ArgumentNullException)
            {
                return Unauthorized("Please login!");                
            }

            if (!_reservationsService.ReservationExists(id))
            {
                return NotFound();
            }

            var reservationServiceResult = await _reservationsService.DeleteReservation(id);
            if (reservationServiceResult.ResponseError != null)
            {
                return BadRequest(reservationServiceResult.ResponseError);
            }

            return NoContent();
        }
    }
}
