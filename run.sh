#!/bin/bash

# Colors & stuff
GREEN="\x1B[32m"
LIGHT_GREEN="\x1B[92m"
NORMAL="\x1B[0m"
YELLOW="\x1B[33m"
UNDERLINED="\x1B[4m"
ARROWS="\x1B[36m>>\x1B[0m"
LIGHT_RED="\x1B[91m"
DONE_TEXT="${LIGHT_GREEN}DONE${NORMAL}"

function show_help() {
    echo    "Usage: $0 [OPTION-1] [OPTION-2] ..."
    echo
    echo    "Options:"
    echo    -e "\t-h, --help             - show help"
    echo    -e "\t-deps                  - run all deps "
    echo    -e "\t -wb                   - run be-warlock "
    echo    -e "\t--ignore-failed-builds - if a build fails, skip it"
    echo    -e "\t--skip-building        - do not build projects"
    echo    -e "\t--show-build-logs      - show gradle build output"
    echo    -e "\t--no-sudo              - do not prefix commands with sudo"
    echo    -e "\t--no-rm                - do not execute 'docker-compose rm -svf'"
    exit 0
}

DEPS_ONLY=true
DOT_NET=true

while [[ $# -gt 0 ]]
do
    key="$1"

    case $key in
        -h|--help)
            show_help
        ;;
        --ignore-failed-builds)
            IGNORE_FAILED_BUILDS=true
            shift
        ;;
        --no-sudo)
            NO_SUDO=true
            shift
        ;;
        --no-rm)
            NO_COMPOSE_RM=true
            shift
        ;;
        -deps)
            DEPS_ONLY=true
            shift
        ;;
        *)
            echo -e "${LIGHT_RED}Error: Unknown option '$1'${NORMAL}"
            exit 1
        ;;
    esac
done

if ! test -f "docker-compose.yaml"; then
    echo -e "${LIGHT_RED}Error: docker-compose.yaml file not found${NORMAL}"
    exit 1
fi

# Projects
WORK_DIR=./
# Services
#MESSENGER_SERVICE=TODO
#DOT_NET_SERVICE=dotnet
POSTGRESQL_SERVICE=database
# sudo isn't needed on MinGW
[[ "$OSTYPE" != "msys" ]] && [[ "$NO_SUDO" != true ]] && SUDO=sudo

function create_dirs() {
  function create_dir_if_needed() {
    [ -d $1 ] || (mkdir -p $1 && echo -e "${ARROWS} Created dir '$1'")
  }
}

# It's a workaround, not a long-term solution
# docker-compose needs all build directories to be valid
create_dirs

# Sets up specified services
function setup() {
    function set_up() {
        echo -e "\n${ARROWS} $SUDO docker-compose -p dotnet up --build -d $@" && $SUDO docker-compose -p dotnet up --build -d "$@" || \
        (echo -e "${LIGHT_RED}Exiting...${NORMAL}" && exit $?)
    }

    set_up $POSTGRESQL_SERVICE
}

[ -z $NO_COMPOSE_RM ] && echo -e "\n${ARROWS} sudo docker-compose rm -svf\n" && $SUDO docker-compose rm -svf
setup # set up all required services
echo -e "dotnet watch run"
[[ $? == '0' ]] && echo -e "\n${DONE_TEXT}"
exit $?