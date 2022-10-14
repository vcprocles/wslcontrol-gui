#!/usr/bin/env perl
use warnings;
use Data::Dumper qw(Dumper);
use File::Copy;

if ($ARGV[0] eq "-i")
{
    print "install mode\n";
    if ($ENV{"USER"} ne "root") 
    {
        print "this mode needs to be run as root\n";
        exit(1);
    }
    print "moving wsl.conf to /etc\n";
    move("wsl.conf","/etc/wsl.conf");
}
else
{
    print "read mode\n";
    if (-e "/etc/wsl.conf")
    {
        print "wsl.conf exists\n";
        $wslconfig_exists=1;
    }
    else
    {
        print "wsl.conf does not exist\n";
        $wslconfig_exists=0;
    }
    if (!$wslconfig_exists)
    {
        print "creating empty wsl.conf\n";
        open my $fh, '>', 'wsl.conf';
        print {$fh} ' ';
        close $fh;
    }
    else
    {
        print "copying wsl.conf from /etc\n";
        copy("/etc/wsl.conf","wsl.conf");
    }
}
print "done.\n";
exit(0);