using System.Collections.Generic;
using System.Linq;

namespace TeamCityBackupTask
{
    public class CompositeBackupValidators : BackupValidator
    {
        private readonly IEnumerable<BackupValidator> _backupValidators;

        public CompositeBackupValidators(IEnumerable<BackupValidator> backupValidators)
        {
            _backupValidators = backupValidators;
        }

        public BackupValidationRecord GetBackupValidation()
        {
            foreach (BackupValidationRecord backupValidationRecord in InvalidBackupValidationRecords()) 
                return backupValidationRecord;

            return BackupValidationRecord.Valid();
        }

        private IEnumerable<BackupValidationRecord> InvalidBackupValidationRecords()
        {
            return _backupValidators.Select(backupValidator => backupValidator.GetBackupValidation())
                                    .Where(backupValidationRecord => !backupValidationRecord.IsValid);
        }
    }
}
