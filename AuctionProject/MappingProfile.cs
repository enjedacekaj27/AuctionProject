using AutoMapper;
using Entities.Dto;
using Entities.Models;

namespace AuctionProject
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<NewAuctionDto, Auction>();
            CreateMap<Auction, AllAuctionDto>();
            CreateMap<Bid, NewBidDto>();
            CreateMap<NewBidDto, Bid>();
        }
    }
}
