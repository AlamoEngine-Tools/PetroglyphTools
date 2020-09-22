namespace PG.Commons.Environment
{
/**
     * Standard exit codes used within bash and C/C++
     * Sources:
     * <list type="bullet">
     *   <item>
     *     <description>
     *       https://tldp.org/LDP/abs/html/exitcodes.html
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *       https://www.apt-browse.org/browse/ubuntu/trusty/main/amd64/libc6-dev/2.19-0ubuntu6/file/usr/include/sysexits.h
     *     </description>
     *   </item>
     * </list>
     */
    public enum ExitCode
    {
        /**
         * The program ran successfully.
         */
        Success = 0, // EX_OK

        /**
         * Miscellaneous errors, such as "divide by zero" and other impermissible operations.
         */
        GenericError = 1,

        /**
         * Missing keyword or command, or permission problem (and diff return code on a failed binary file comparison).
         */
        MisusedBuiltinError = 2,

        /**
         * EX_USAGE -- The command was used incorrectly, e.g., with the wrong number of arguments, a bad flag,
         * a bad syntax in a parameter, or whatever.
         */
        ExUsage = 64,

        /**
         * EX_DATAERR -- The input data was incorrect in some way. This should only be used for user's data
         * and not system files.
         */
        ExDataerr = 65,

        /**
         * EX_NOINPUT -- An input file (not a system file) did not exist or was not readable.
         * This could also include errors like "No message" to a mailer (if it cared to catch it).
         */
        ExNoinput = 66,

        /**
         * EX_NOUSER -- The user specified did not exist. This might be used for mail addresses or remote logins.
         */
        ExNouser = 67,

        /**
         * EX_NOHOST -- The host specified did not exist. This is used in mail addresses or network requests.
         */
        ExNohost = 68,

        /**
         * EX_UNAVAILABLE -- A service is unavailable. This can occur if a support program or file does not exist.
         * This can also be used as a catchall message when something you wanted to do doesn't work,
         * but you don't know why.
         */
        ExUnavailable = 69,

        /**
         * EX_SOFTWARE -- An internal software error has been detected. This should be limited to
         * non-operating system related errors as possible.
         */
        ExSoftware = 70,

        /**
         * EX_OSERR -- An operating system error has been detected. This is intended to be used for such things
         * as "cannot fork", "cannot create pipe", or the like. It includes things like getuid
         * returning a user that does not exist in the passwd file.
         */
        ExOserr = 71,

        /**
         * EX_OSFILE -- Some system file (e.g., /etc/passwd, /etc/utmp, etc.) does not exist, cannot be opened,
         * or has some sort of error (e.g., syntax error).
         */
        ExOsfile = 72,

        /**
         * EX_CANTCREAT -- A (user specified) output file cannot be created.
         */
        ExCantcreat = 73,

        /**
         * EX_IOERR -- An error occurred while doing I/O on some file.
         */
        ExIoerr = 74,

        /**
         * EX_TEMPFAIL -- temporary failure, indicating something that is not really an error.
         * In sendmail, this means that a mailer (e.g.) could not create a connection, and the request
         * should be reattempted later.
         */
        ExTempfail = 75,

        /**
         * EX_PROTOCOL -- the remote system returned something that was "not possible" during a protocol exchange.
         */
        ExProtocol = 76,

        /**
         * EX_NOPERM -- You did not have sufficient permission to perform the operation.
         * This is not intended for file system problems, which should use NOINPUT or CANTCREAT,
         * but rather for higher level permissions.
         */
        ExNoperm = 77,

        /**
         * EX_CONFIG -- An error ina required configuration (file).
         */
        ExConfig = 78,

        /**
         * Permission problem or command is not an executable
         */
        CommandCannotExecuteError = 126,

        /**
         * Possible problem with $PATH or a typo
         */
        CommandNotFoundError = 127,

        /**
         * exit takes only integer args in the range 0 - 255
         */
        InvalidExitArgumentError = 128,

        // 128+n Reserved for fatal error signals, e.g. sigkill
        /**
         * User aborted with Control+C, Control-C is fatal error signal 2, (130 = 128 + 2, see above)
         */
        UserAbortedError = 130,
        /**
         * Catchall for high error codes.
         */
        ExitOutOfRangeError = 255,
        // Reserved ranges:
        // 0-2, 64 - 78, 126 - 165, 255
    }
}