#!/usr/bin/env perl
use strict;
use warnings;
use Data::Dumper qw(Dumper);
use File::Copy;

if ($ARGV[0] eq "-i")
{
    print "install mode\n";
    #cleaning the file
    open(FILE, "<wsl.conf") || die "where's my wsl.conf?";
    my @lines = <FILE>;
    close(FILE);
    foreach(@lines) {
        $_ =~ s/\015\012/\012/g;
        $_ =~ s/"true"/true/g;
        $_ =~ s/"false"/false/g;
    }
    open(FILE,">wsl.conf") || die "where's my wsl.conf? i read from it just now";
    print FILE @lines;
    close (FILE);
    #installing the file
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
    our $wslconfig_exists=0;
    print "read mode\n";
    print "checking for existing copy of wsl.conf in ~\n";
    if (-e "wsl.conf")
    {
        print "local wsl.conf exists, removing\n";
        unlink "wsl.conf";
    }
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
