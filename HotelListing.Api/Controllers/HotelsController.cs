using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data.Models;
using HotelListing.API.Repository.Contracts;
using AutoMapper;
using HotelListing.API.Commos.Domains.Hotel;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;
        public HotelsController(IHotelRepository hotelRepository, IMapper mapper)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
        }

        // GET: api/Hotels
        [HttpGet]
        public async Task<IActionResult> GetHotels()
        {
           var hotels = await _hotelRepository.GetAllAsync();
           var records = _mapper.Map<IEnumerable<HotelDto>>(hotels);
            return Ok(records);
        }

        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHotel(int id)
        {
            var hotel = await _hotelRepository.GetAsync(id);

            if (hotel == null)
            {
                return NotFound();
            }
            var hotelDto = _mapper.Map<HotelDto>(hotel);

            return Ok(hotelDto);
        }

        // PUT: api/Hotels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, HotelDto UpdateHotelDto)
        {
            if (id != UpdateHotelDto.Id)
            {
                return BadRequest();
            }

            var hotel = await _hotelRepository.GetAsync(id);

            _mapper.Map(UpdateHotelDto, hotel);
            try
            {
                await _hotelRepository.UpdateAsync(hotel);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await HotelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Hotels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Hotel>> PostHotel(CreateHotelDto CreateHotel)
        {
            var hotel = _mapper.Map<Hotel>(CreateHotel);    
           await _hotelRepository.AddAsync(hotel);

            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
           var hotel = await _hotelRepository.GetAsync(id);

            if (hotel == null)
            {
                return NotFound();
            }

            await _hotelRepository.Deletesync(id);
            return NoContent();
        }

        private async Task<bool> HotelExists(int id)
        {
            return await _hotelRepository.Exists(id);
        }
    }
}
