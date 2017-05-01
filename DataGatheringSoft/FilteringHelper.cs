using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataGatheringSoft.Models;

namespace DataGatheringSoft
{
    public static class FilteringHelper
    {
        public static IEnumerable<FileModel> Filter(IEnumerable<FileModel> allFiles, Views.OptionsViewModel OptionsVM)
        {
            var filtered = allFiles;
            if (!String.IsNullOrWhiteSpace(OptionsVM.Author))
            {
                var author = OptionsVM.Author.ToLower();
                filtered = filtered.Where(x => x.Author.ToLower() == author);
            }

            if (OptionsVM.AccessDateFrom.HasValue)
                filtered = filtered.Where(x => x.AccessDate > OptionsVM.AccessDateFrom.Value);
            if (OptionsVM.AccessDateTo.HasValue)
                filtered = filtered.Where(x => x.AccessDate < OptionsVM.AccessDateTo.Value);

            if (OptionsVM.CreationDateFrom.HasValue)
                filtered = filtered.Where(x => x.CreationDate > OptionsVM.CreationDateFrom.Value);
            if (OptionsVM.CreationDateTo.HasValue)
                filtered = filtered.Where(x => x.CreationDate < OptionsVM.CreationDateTo.Value);

            if (OptionsVM.ModificationDateFrom.HasValue)
                filtered = filtered.Where(x => x.ModificationDate > OptionsVM.ModificationDateFrom.Value);
            if (OptionsVM.ModificationDateTo.HasValue)
                filtered = filtered.Where(x => x.ModificationDate < OptionsVM.ModificationDateTo.Value);

            if (!String.IsNullOrEmpty(OptionsVM.SizeFrom))
            {
                var size = Int64.Parse(OptionsVM.SizeFrom);
                filtered = filtered.Where(x => x.Size > size);
            }
            if (!String.IsNullOrEmpty(OptionsVM.SizeTo))
            {
                var size = Int64.Parse(OptionsVM.SizeTo);
                filtered = filtered.Where(x => x.Size > size);
            }

            if (!String.IsNullOrWhiteSpace(OptionsVM.Extensions))
            {
                var splitted = OptionsVM.Extensions.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var exts = splitted.Select(x => new StringBuilder().Append('.').Append(x.Trim()).ToString()).Where(x => !String.IsNullOrEmpty(x)).ToList();

                filtered = filtered.Where(x => exts.Contains(x.Extension));
            }

            if (!String.IsNullOrWhiteSpace(OptionsVM.Name))
            {
                var name = OptionsVM.Name.ToLower();
                filtered = filtered.Where(x => x.Name.ToLower().Contains(name));
            }

            return filtered;
        }
    }
}
