using Hangfire;
using HangFireDemo.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace HangFireDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : Controller
    {
        [HttpPost]
        [Route("CreateBackgroundJob")]
        public IActionResult CreateBackgroudJob()
        {
            // Fire-and-Forget type of Job => ex. sending welcome email to the newly registerd user
            // eg. informative jobs, don't have to wait for result
            BackgroundJob.Enqueue<TestJob>(x => x.WriteLog("BackgroundJob Triggered"));
            return Ok();
        }

        [HttpPost]
        [Route("CreateScheduledJob")]
        public IActionResult CreateScheduledJob()
        {
            var scheduledDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduledDateTime);

            // Scheduled jobs => ex.scheduling job (only once) in specific time
            // eg. run after declared time, regurly. This job is triggered every 5 seconds. 
            // First parameter is method, and second after which time job is triggered
            BackgroundJob.Schedule<TestJob>(x => x.WriteLog("BackgroundJob Triggered"), dateTimeOffset);
            return Ok();
        }

        [HttpPost]
        [Route("CreateContinuatingJob")]
        public IActionResult CreateContinuatingJob()
        {
            var scheduledDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduledDateTime);

            var jobId = BackgroundJob.Schedule(() => Console.WriteLine("Continuating HangFire Job Triggered"), dateTimeOffset);

            // Continues-Job is job that continues after different job.
            // Methods Schedule(), Enqueue() etc.. returns Id of the job
            //First parameter says, after which job with these Id should my Continues-Job continue
            //Second pameter says which method should my Continues-Job triggere
            var jobId2 = BackgroundJob.ContinueJobWith<TestJob>(jobId, x => x.WriteLog("Continues-Job 1 is Triggered"));
            var jobId3 = BackgroundJob.ContinueJobWith<TestJob>(jobId2, x => x.WriteLog("Continues-Job 2 is Triggered"));
            BackgroundJob.ContinueJobWith<TestJob>(jobId3, x => x.WriteLog("Continues-Job 3 is Triggered"));

            return Ok();
        }

        [HttpPost]
        [Route("CreateReccuringJob")]
        public IActionResult CreateReccuringJob()
        {
            RecurringJob.AddOrUpdate<TestJob>("ReccuringJob1", x => x.WriteLog("ReccuringJob1 is Triggered"), "* * * * * *");
            return Ok();
        }
    }
}
