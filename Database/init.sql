/****** Object:  Database [StrengthIgniter]    Script Date: 12/05/2020 12:09:56 ******/
CREATE DATABASE [StrengthIgniter]

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [StrengthIgniter].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [StrengthIgniter] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [StrengthIgniter] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [StrengthIgniter] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [StrengthIgniter] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [StrengthIgniter] SET ARITHABORT OFF 
GO

ALTER DATABASE [StrengthIgniter] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [StrengthIgniter] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [StrengthIgniter] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [StrengthIgniter] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [StrengthIgniter] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [StrengthIgniter] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [StrengthIgniter] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [StrengthIgniter] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [StrengthIgniter] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [StrengthIgniter] SET  DISABLE_BROKER 
GO

ALTER DATABASE [StrengthIgniter] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [StrengthIgniter] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [StrengthIgniter] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [StrengthIgniter] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [StrengthIgniter] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [StrengthIgniter] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [StrengthIgniter] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [StrengthIgniter] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [StrengthIgniter] SET  MULTI_USER 
GO

ALTER DATABASE [StrengthIgniter] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [StrengthIgniter] SET DB_CHAINING OFF 
GO

ALTER DATABASE [StrengthIgniter] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [StrengthIgniter] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [StrengthIgniter] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [StrengthIgniter] SET QUERY_STORE = OFF
GO

ALTER DATABASE [StrengthIgniter] SET  READ_WRITE 
GO


