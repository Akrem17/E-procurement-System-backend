﻿using E_proc.Models;

namespace E_proc.Repositories.Interfaces
{
    public interface IInstituteRepository
    {
        Task<IEnumerable<Institute>> ReadAsync();

        Task<Institute> CreateAsync(Institute institute);

        Task<Institute> ReadById(int id);

        Task<Institute> UpdateAsync(int id, Institute institute);
        Task<List<Institute>> FindBy(string? email, bool? confirmed, string? phone,DateTime? date);

        Task<int> Delete(int id);
        Task<IEnumerable<Tender>> getTendersOfInstitute(int id, int skip, int take);
        Task<int> getTendersOfInstituteCountData(int id);
       // Task<FileData[]> getInstituteSpecifications(int id);




    }
}
