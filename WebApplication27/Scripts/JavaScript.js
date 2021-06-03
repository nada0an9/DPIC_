//only number function
function validateNumber(e)
{
    const pattern = /^[0-9]$/;

    return pattern.test(e.key)
}
