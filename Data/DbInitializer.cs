using CollegeWebSite.Data;
using CollegeWebSite.Domain.Entities;
using CollegeWebSite.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CollegeWebSite.Data;

public interface IDbInitializer
{
    Task InitializeAsync();
}

public class DbInitializer : IDbInitializer
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<DbInitializer> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        try
        {
            // Create database and apply migrations
            await context.Database.EnsureCreatedAsync();

            // Seed Page Templates
            if (!await context.PageTemplates.AnyAsync())
            {
                var defaultTemplate = new PageTemplate
                {
                    Name = "Default Layout",
                    Description = "Standard single column article layout",
                    LayoutJson = "{}",
                    IsSystemTemplate = true,
                    CreatedOn = DateTime.UtcNow
                };
                context.PageTemplates.Add(defaultTemplate);
                await context.SaveChangesAsync();
            }

            // Seed Basic Pages
            if (!await context.PageContents.AnyAsync())
            {
                var pages = new List<PageContent>
                {
                    new PageContent
                    {
                        Title = "About Ishika Studies",
                        Slug = "about-us",
                        ContentData = @"
                            <p>Established with a vision to redefine nursing education, <strong>Ishika Nursing and Paramedical Studies</strong> stands as a pillar of academic excellence in Kendujhar, Odisha. Our institution is dedicated to bridging the gap between theoretical medical knowledge and hands-on clinical precision.</p>
                            <h2>Our Clinical Vision</h2>
                            <p>We believe that nursing is not just a profession, but a sacred calling. Our curriculum is designed to foster not only technical mastery but also deep human compassion. Students at Ishika are trained in multi-specialty environments, ensuring they are ready for global healthcare challenges.</p>
                            <img src='https://images.unsplash.com/photo-1576091160550-2173bdb999ef?auto=format&fit=crop&q=80&w=1200' alt='Campus' />
                            <h2>Core Mission</h2>
                            <ul>
                                <li>To provide world-class paramedical training at affordable costs.</li>
                                <li>To ensure 100% placement for every graduating student.</li>
                                <li>To foster innovation in rural healthcare delivery.</li>
                            </ul>",
                        IsPublished = true,
                        CreatedOn = DateTime.UtcNow
                    },
                    new PageContent
                    {
                        Title = "GNM Nursing Diploma",
                        Slug = "gnm-nursing",
                        ContentData = @"
                            <p>The <strong>General Nursing and Midwifery (GNM)</strong> course at Ishika is a rigorous 3-year diploma program recognized by the <strong>ONMRC</strong>. It is meticulously crafted to prepare students for the demands of modern hospital services.</p>
                            <h3>Course Highlights</h3>
                            <p>Our GNM track focuses on community health, maternal care, and surgical assistance. With over 2000+ hours of clinical rotation, our students become the preferred choice for top-tier hospitals.</p>
                            <blockquote>
                                'Ishika's GNM program gave me the confidence to handle emergency situations in high-pressure environments.' - Alumni, Batch 2022
                            </blockquote>
                            <h3>Eligibility Criteria</h3>
                            <ul>
                                <li>Minimum 17 years of age.</li>
                                <li>10+2 with 40% aggregate marks (Science preferred).</li>
                                <li>Physical and mental fitness as per INC norms.</li>
                            </ul>",
                        IsPublished = true,
                        CreatedOn = DateTime.UtcNow
                    }
                };
                context.PageContents.AddRange(pages);
                await context.SaveChangesAsync();
            }

            // Seed Gallery
            if (!await context.GalleryCategories.AnyAsync())
            {
                var labCategory = new GalleryCategory { Name = "Medical Labs", Description = "Advanced nursing simulation and anatomy labs.", CreatedOn = DateTime.UtcNow };
                var campusCategory = new GalleryCategory { Name = "Infrastructure", Description = "Our modern campus and clinical facilities.", CreatedOn = DateTime.UtcNow };
                var studentCategory = new GalleryCategory { Name = "Student Life", Description = "Cultural programs and student activities.", CreatedOn = DateTime.UtcNow };

                context.GalleryCategories.AddRange(labCategory, campusCategory, studentCategory);
                await context.SaveChangesAsync();

                var images = new List<GalleryImage>
                {
                    new GalleryImage {
                        Title = "Advanced Anatomy Lab",
                        Description = "Equipped with high-fidelity manikins for hands-on anatomy training.",
                        GalleryCategoryId = labCategory.Id,
                        Media = new Media { Url = "https://images.unsplash.com/photo-1579154235602-3c2cff25b507?auto=format&fit=crop&q=80&w=800", AltText = "Anatomy Lab", CreatedOn = DateTime.UtcNow },
                        DisplayOrder = 1,
                        CreatedOn = DateTime.UtcNow
                    },
                    new GalleryImage {
                        Title = "Nursing Simulation Center",
                        Description = "Students practicing emergency response in a simulated hospital ward.",
                        GalleryCategoryId = labCategory.Id,
                        Media = new Media { Url = "https://images.unsplash.com/photo-1551076805-e1869033e561?auto=format&fit=crop&q=80&w=800", AltText = "Simulation Center", CreatedOn = DateTime.UtcNow },
                        DisplayOrder = 2,
                        CreatedOn = DateTime.UtcNow
                    },
                    new GalleryImage {
                        Title = "Main Academic Block",
                        Description = "Our state-of-the-art academic building in Kendujhar.",
                        GalleryCategoryId = campusCategory.Id,
                        Media = new Media { Url = "https://images.unsplash.com/photo-1562774053-701939374585?auto=format&fit=crop&q=80&w=800", AltText = "Academic Block", CreatedOn = DateTime.UtcNow },
                        DisplayOrder = 1,
                        CreatedOn = DateTime.UtcNow
                    },
                    new GalleryImage {
                        Title = "Digital Library",
                        Description = "Access to global medical research with high-speed campus internet.",
                        GalleryCategoryId = campusCategory.Id,
                        Media = new Media { Url = "https://images.unsplash.com/photo-1521587760476-6c12a4b040da?auto=format&fit=crop&q=80&w=800", AltText = "Digital Library", CreatedOn = DateTime.UtcNow },
                        DisplayOrder = 2,
                        CreatedOn = DateTime.UtcNow
                    },
                    new GalleryImage {
                        Title = "Annual Sports Meet",
                        Description = "Fostering teamwork and physical health among our nursing students.",
                        GalleryCategoryId = studentCategory.Id,
                        Media = new Media { Url = "https://images.unsplash.com/photo-1526676037777-05a232554f77?auto=format&fit=crop&q=80&w=800", AltText = "Sports Meet", CreatedOn = DateTime.UtcNow },
                        DisplayOrder = 1,
                        CreatedOn = DateTime.UtcNow
                    }
                };
                context.GalleryImages.AddRange(images);
                await context.SaveChangesAsync();
            }

            // Seed Faculty
            if (!await context.Faculty.AnyAsync())
            {
                var facultyMembers = new List<Faculty>
                {
                    new Faculty
                    {
                        Name = "Prof. Sumati Mohapatra",
                        Designation = "Principal",
                        Qualification = "M.Sc Nursing, PhD",
                        Email = "principal@ishika.edu.in",
                        DisplayOrder = 1,
                        Bio = "Over 25 years of experience in academic nursing leadership and clinical excellence.",
                        Photo = new Media { Url = "https://images.unsplash.com/photo-1559839734-2b71f1536785?auto=format&fit=crop&q=80&w=400", FileName = "principal.jpg", CreatedOn = DateTime.UtcNow },
                        CreatedOn = DateTime.UtcNow
                    },
                    new Faculty
                    {
                        Name = "Dr. Rajesh Kumar",
                        Designation = "Vice-Principal",
                        Qualification = "M.Sc Nursing (Medical Surgical)",
                        Email = "vprincipal@ishika.edu.in",
                        DisplayOrder = 2,
                        Bio = "Specialist in critical care nursing with multiple research publications in national journals.",
                        Photo = new Media { Url = "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?auto=format&fit=crop&q=80&w=400", FileName = "vp.jpg", CreatedOn = DateTime.UtcNow },
                        CreatedOn = DateTime.UtcNow
                    },
                    new Faculty
                    {
                        Name = "Mrs. Sunita Nayak",
                        Designation = "Senior Tutor",
                        Qualification = "M.Sc Nursing (OBG)",
                        Email = "sunita@ishika.edu.in",
                        DisplayOrder = 3,
                        Bio = "Experienced educator focusing on maternal and child health nursing practices.",
                        Photo = new Media { Url = "https://images.unsplash.com/photo-1594824813511-28adba291d1b?auto=format&fit=crop&q=80&w=400", FileName = "tutor1.jpg", CreatedOn = DateTime.UtcNow },
                        CreatedOn = DateTime.UtcNow
                    }
                };
                context.Faculty.AddRange(facultyMembers);
                await context.SaveChangesAsync();
            }

            // Seed Events
            if (!await context.Events.AnyAsync())
            {
                var events = new List<Event>
                {
                    new Event
                    {
                        Title = "National Nursing Workshop 2025",
                        Description = "A comprehensive workshop on advanced clinical practices and patient care technologies in modern healthcare.",
                        EventDate = DateTime.UtcNow.AddDays(15),
                        Venue = "Main Auditorium, Ishika Campus",
                        IsPublished = true,
                        PublishedOn = DateTime.UtcNow,
                        Banner = new Media { Url = "https://images.unsplash.com/photo-1540575467063-178a50c2df87?auto=format&fit=crop&q=80&w=800", FileName = "workshop.jpg", CreatedOn = DateTime.UtcNow },
                        CreatedOn = DateTime.UtcNow
                    },
                    new Event
                    {
                        Title = "Fresher's Orientation Program",
                        Description = "Welcome ceremony and orientation for the 2025-26 batch of nursing and paramedical students.",
                        EventDate = DateTime.UtcNow.AddDays(30),
                        Venue = "Campus Open Grounds",
                        IsPublished = true,
                        PublishedOn = DateTime.UtcNow,
                        Banner = new Media { Url = "https://images.unsplash.com/photo-1523580494863-6f3031224c94?auto=format&fit=crop&q=80&w=800", FileName = "orientation.jpg", CreatedOn = DateTime.UtcNow },
                        CreatedOn = DateTime.UtcNow
                    },
                    new Event
                    {
                        Title = "Community Health Camp",
                        Description = "Our students participating in rural health awareness and primary checkup campaigns in Kendujhar district.",
                        EventDate = DateTime.UtcNow.AddDays(-10),
                        Venue = "Kendujhar Rural Health Center",
                        IsPublished = true,
                        PublishedOn = DateTime.UtcNow,
                        Banner = new Media { Url = "https://images.unsplash.com/photo-1576091160550-2173dba999ef?auto=format&fit=crop&q=80&w=800", FileName = "health-camp.jpg", CreatedOn = DateTime.UtcNow },
                        CreatedOn = DateTime.UtcNow
                    }
                };
                context.Events.AddRange(events);
                await context.SaveChangesAsync();
            }

            // Seed Placements
            if (!await context.PlacementRecords.AnyAsync())
            {
                var placements = new List<PlacementRecord>
                {
                    new PlacementRecord
                    {
                        StudentName = "Anjali Sahoo",
                        Course = "GNM Nursing",
                        HospitalName = "Apollo Hospitals, Bhubaneswar",
                        Designation = "Staff Nurse",
                        Batch = "2020-23",
                        IsFeatured = true,
                        DisplayOrder = 1,
                        Testimonial = "Ishika provided me with the clinical confidence and theoretical depth required to work at a premier healthcare facility like Apollo.",
                        StudentPhoto = new Media { Url = "https://images.unsplash.com/photo-1594824813511-28adba291d1b?auto=format&fit=crop&q=80&w=400", FileName = "alumni1.jpg", CreatedOn = DateTime.UtcNow },
                        CreatedOn = DateTime.UtcNow
                    },
                    new PlacementRecord
                    {
                        StudentName = "Rahul Mahanta",
                        Course = "Medical Lab Technician",
                        HospitalName = "AMRI Hospitals",
                        Designation = "Senior Lab Technician",
                        Batch = "2021-23",
                        IsFeatured = true,
                        DisplayOrder = 2,
                        Testimonial = "The hands-on training labs at Ishika are exceptional. I felt ready for the clinical environment from day one.",
                        StudentPhoto = new Media { Url = "https://images.unsplash.com/photo-1622253692010-333f2da6031d?auto=format&fit=crop&q=80&w=400", FileName = "alumni2.jpg", CreatedOn = DateTime.UtcNow },
                        CreatedOn = DateTime.UtcNow
                    },
                    new PlacementRecord
                    {
                        StudentName = "Sujata Sethi",
                        Course = "ANM Nursing",
                        HospitalName = "Fortis Health Management",
                        Designation = "Hospice Nurse",
                        Batch = "2021-23",
                        IsFeatured = true,
                        DisplayOrder = 3,
                        Testimonial = "Compassion is taught alongside clinical skills here. I'm proud to be an Ishika alumna.",
                        StudentPhoto = new Media { Url = "https://images.unsplash.com/photo-1559839734-2b71f1536785?auto=format&fit=crop&q=80&w=400", FileName = "alumni3.jpg", CreatedOn = DateTime.UtcNow },
                        CreatedOn = DateTime.UtcNow
                    }
                };
                context.PlacementRecords.AddRange(placements);
                await context.SaveChangesAsync();
            }

            // Seed Student Results
            if (!await context.StudentResults.AnyAsync())
            {
                var results = new List<StudentResult>
                {
                    new StudentResult
                    {
                        RollNumber = "ISH2024001",
                        StudentName = "Priyanka Mohanty",
                        ExamName = "GNM 1st Year Annual Exam 2024",
                        Course = "GNM Nursing",
                        Batch = "2023-26",
                        ResultStatus = "Pass",
                        TotalPercentage = 84.5,
                        Grade = "A+",
                        Remarks = "Excellent academic performance in clinical and theory.",
                        IsPublished = true,
                        CreatedOn = DateTime.UtcNow
                    },
                    new StudentResult
                    {
                        RollNumber = "ISH2024002",
                        StudentName = "Rahul Dash",
                        ExamName = "GNM 1st Year Annual Exam 2024",
                        Course = "GNM Nursing",
                        Batch = "2023-26",
                        ResultStatus = "Pass",
                        TotalPercentage = 78.2,
                        Grade = "A",
                        Remarks = "Consistent performance. Good clinical skills.",
                        IsPublished = true,
                        CreatedOn = DateTime.UtcNow
                    }
                };
                context.StudentResults.AddRange(results);
                await context.SaveChangesAsync();
            }

            // Seed Campus Notices
            if (!await context.CampusNotices.AnyAsync())
            {
                var notices = new List<CampusNotice>
                {
                    new CampusNotice
                    {
                        Title = "GNM Admission Deadline Extended!",
                        Content = "The last date for GNM Nursing admissions 2025-26 has been extended to August 30th. Apply now to secure your seat.",
                        IsUrgent = true,
                        LinkUrl = "/apply",
                        CreatedOn = DateTime.UtcNow
                    },
                    new CampusNotice
                    {
                        Title = "Summer Vacation Announcement",
                        Content = "Academic sessions will remain suspended from June 1st to June 15th for summer vacation. Hostel remains open.",
                        IsUrgent = false,
                        CreatedOn = DateTime.UtcNow
                    }
                };
                context.CampusNotices.AddRange(notices);
                await context.SaveChangesAsync();
            }

            // Seed Syllabuses
            if (!await context.Syllabuses.AnyAsync())
            {
                var syllabuses = new List<Syllabus>
                {
                    new Syllabus
                    {
                        CourseName = "GNM Nursing",
                        Subject = "Anatomy & Physiology",
                        AcademicYear = "1st Year",
                        Description = "Fundamentals of human anatomy, skeletal system, and physiological processes.",
                        DisplayOrder = 1,
                        CreatedOn = DateTime.UtcNow
                    },
                    new Syllabus
                    {
                        CourseName = "GNM Nursing",
                        Subject = "Community Health Nursing",
                        AcademicYear = "1st Year",
                        Description = "Principles of public health, epidemiology, and rural healthcare delivery.",
                        DisplayOrder = 2,
                        CreatedOn = DateTime.UtcNow
                    },
                    new Syllabus
                    {
                        CourseName = "ANM",
                        Subject = "Child Health Nursing",
                        AcademicYear = "2nd Year",
                        Description = "Pediatric care, immunization schedules, and infant nutrition management.",
                        DisplayOrder = 1,
                        CreatedOn = DateTime.UtcNow
                    }
                };
                context.Syllabuses.AddRange(syllabuses);
                await context.SaveChangesAsync();
            }
            
            // Seed Hero Content
            if (!await context.HeroSectionContent.AnyAsync())
            {
                var hero = new HeroContent
                {
                    WelcomeBadge = "ADMISSIONS OPEN",
                    HeadlineMain = "Crafting the Future of Care",
                    HeadlineShine = "at Ishika Campus",
                    SubHeadline = "Step into a world of clinical precision and compassionate service. Odisha's top-rated health education institution located in Kendujhar.",
                    PrimaryButtonText = "Begin Application",
                    PrimaryButtonLink = "/apply",
                    SecondaryButtonText = "Explore Curriculum",
                    SecondaryButtonLink = "/p/courses",
                    Stat1Value = "500+",
                    Stat1Label = "Elite Alumni",
                    Stat2Value = "100%",
                    Stat2Label = "Placement",
                    Stat3Value = "A+ Grade",
                    Stat3Label = "ONMRC",
                    BadgeStatus = "Authentic",
                    BadgeLabel = "Govt Recognized",
                    BannerImagePath = "images/banner.jpeg",
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow
                };
                context.HeroSectionContent.Add(hero);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}
