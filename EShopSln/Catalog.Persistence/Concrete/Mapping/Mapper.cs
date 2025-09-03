using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AM = AutoMapper; // alias
using IMapper = Catalog.Application.Interfaces.Mapping.IMapper;

public sealed class Mapper : IMapper
{
    private readonly AM.IMapper _mapper;
    public Mapper(AM.IMapper mapper) => _mapper = mapper;

    public TDest Map<TDest, TSrc>(TSrc src)
        => _mapper.Map<TSrc, TDest>(src);

    public IList<TDest> Map<TDest, TSrc>(IEnumerable<TSrc> src)
        => _mapper.Map<List<TDest>>(src); // caller IList olarak tutabilir

    public TDest Map<TDest>(object src)
        => _mapper.Map<TDest>(src);
}