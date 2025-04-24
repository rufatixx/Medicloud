function parseTime(timeStr) {
    const [hour, minute] = timeStr.split(":").map(Number);
    return hour * 60 + minute;
}

function formatTime(minutes) {
    const hour = Math.floor(minutes / 60);
    const minute = minutes % 60;
    return `${String(hour).padStart(2, '0')}:${String(minute).padStart(2, '0')}`;
}

function isInRange(startA, endA, startB, endB) {
    return !(endA <= startB || startA >= endB);
}

function getSlotStatus(startTime, endTime, breaks, reserved) {
    for (const [start, end] of breaks) {
        if (isInRange(startTime, endTime, parseTime(start), parseTime(end))) return "break";
    }
    for (const [start, end] of reserved) {
        if (isInRange(startTime, endTime, parseTime(start), parseTime(end))) return "reserved";
    }
    return "available";
}

function generateTimeSlots(start, end, interval, breaks, reserved) {
    const slots = [];
    let current = parseTime(start);
    const endTime = parseTime(end);

    while (current + interval <= endTime) {
        const slotEnd = current + interval;
        const label = `${formatTime(current)} - ${formatTime(slotEnd)}`;
        const status = getSlotStatus(current, slotEnd, breaks, reserved);
        slots.push({ label, status });
        current += interval;
    }

    return slots;
}
