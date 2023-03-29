using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data.Models;
using HotelListing.API.Commos.Domains.Country;
using AutoMapper;
using HotelListing.API.Repository.Contracts;
using Microsoft.AspNetCore.Authorization;
using HotelListing.API.Handlers.Exceptions;
using HotelListing.API.Commos.Domains.Pagging;
using Microsoft.AspNetCore.OData.Query;

namespace HotelListing.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;


        public CountriesController(IMapper mapper,ICountriesRepository countriesRepository)
        {
            _mapper = mapper;
            _countriesRepository = countriesRepository;
        }

        // GET: api/Countries
        
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _countriesRepository.GetAllAsync();
            var records = _mapper.Map<IEnumerable<GetCountryDto>>(countries);
            return Ok(records);
        }

        [HttpGet("GetAllOData")]
        [EnableQuery]
        [Authorize]
        public async Task<IActionResult> GetODataCountries()
        {
            var countries = await _countriesRepository.GetAllAsync();
            var records = _mapper.Map<IEnumerable<GetCountryDto>>(countries);
            return Ok(records);
        }



        // GET: api/Countries/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCountry(int id)
        {
        
            var country = await _countriesRepository.GetCountryDetails(id);

            if(country == null)
            {
                throw new NotFoundException(nameof(GetCountries), id);
            }


            var countryDto = _mapper.Map<CountryDto>(country);

            return Ok(countryDto);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPagedCountries([FromQuery] QueryParameters queryParameters)
        {
            var pagedCountriesResult = await _countriesRepository.GetAllPagedAsync<GetCountryDto>(queryParameters);
            
            return Ok(pagedCountriesResult);
        }



        // PUT: api/Countries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
        {
            if (id != updateCountryDto.Id) return BadRequest("Invalid Record Id");

            //_context.Entry(updateCountryDto).State = EntityState.Modified;

            var country = await _countriesRepository.GetAsync(id);


            if (country == null)
            {
                throw new NotFoundException(nameof(PutCountry), id);
            }

            _mapper.Map(updateCountryDto,country);

            try
            {
                await _countriesRepository.UpdateAsync(country);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExists(id)) return NotFound();
                else throw;
            }
            return NoContent();
        }





        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountryDto)
        {

            var country = _mapper.Map<Country>(createCountryDto);



            await _countriesRepository.AddAsync(country);

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }








        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        
        public async Task<IActionResult> DeleteCountry(int id)
        {
          
            var country = await _countriesRepository.GetAsync(id);

            if (country == null) throw new NotFoundException(nameof(DeleteCountry), id);

            await _countriesRepository.Deletesync(id);

            return NoContent();
        }

        private async Task<bool> CountryExists(int id) 
                                => await _countriesRepository.Exists(id);
        
    }
}
