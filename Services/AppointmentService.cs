using System;
using System.Linq;
using System.Threading.Tasks;
using HUTECH_Hospital.Models;
using HUTECH_Hospital.Repositories;

namespace HUTECH_Hospital.Services
{
    public class AppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IDoctorScheduleRepository _scheduleRepo;

        public AppointmentService(IAppointmentRepository appointmentRepo, IDoctorScheduleRepository scheduleRepo)
        {
            _appointmentRepo = appointmentRepo;
            _scheduleRepo = scheduleRepo;
        }

        public async Task<bool> CanBookAppointmentAsync(int doctorId, DateTime date, TimeSpan time, int? excludeId = null)
        {
            // 1. Không đặt quá khứ
            var appointmentDateTime = date.Date.Add(time);
            if (appointmentDateTime < DateTime.Now)
                return false;

            // 2. Không trùng lịch bác sĩ
            bool isBooked = await _appointmentRepo.IsDoctorTimeSlotBookedAsync(doctorId, date, time, excludeId);
            if (isBooked) return false;

            // 3. Phải nằm trong lịch làm việc nếu bác sĩ có xếp ca
            return await IsWithinDoctorScheduleAsync(doctorId, date, time);
        }

        public async Task<bool> IsWithinDoctorScheduleAsync(int doctorId, DateTime date, TimeSpan time)
        {
            var schedules = await _scheduleRepo.GetByDoctorIdAsync(doctorId);
            var dailySchedules = schedules.Where(s => s.WorkDate.Date == date.Date && s.IsAvailable).ToList();

            // Nếu bác sĩ không có lịch làm việc cố định ngày đó, tuỳ theo logic bệnh viện.
            // Trong đề bài: "Nếu có DoctorSchedule thì thời gian đặt phải nằm trong lịch làm việc".
            if (!dailySchedules.Any()) return true; // Giả sử cho phép nếu Admin chưa xếp lịch (tuỳ quy tắc). HOẶC false. Đề bài ko nói chặn đứng. Ở đây cho logic fallback là pass hoặc fail tuỳ hỉ. Để strict, trả về fail.
            
            // Xử lý nghiêm ngặt: Phải nằm trong khung schedule
            foreach (var s in dailySchedules)
            {
                if (time >= s.StartTime && time <= s.EndTime)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CanEditOrCancelAsync(Appointment appointment)
        {
            return appointment.Status == "Pending" || appointment.Status == "Confirmed";
        }
    }
}
