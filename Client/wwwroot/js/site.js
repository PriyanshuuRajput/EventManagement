window.initializeCalendar = (eventDates) => {
    if (!eventDates) eventDates = [];

    const todayStr = new Date().toISOString().split('T')[0];

    flatpickr("#datepicker", {
        inline: true,
        weekStart: 1,
        defaultDate: todayStr,
        disableMobile: true,
        onDayCreate: function (dObj, dStr, fp, dayElem) {
            const dateStr = dayElem.dateObj.toISOString().split('T')[0];

            // Clear existing classes
            dayElem.classList.remove(
                'bg-primary', 'text-white',
                'bg-secondary', 'text-dark',
                'border', 'border-danger', 'text-danger',
                'rounded-circle', 'fw-bold'
            );

            // Event dates -> danger outline
            if (eventDates.includes(dateStr)) {
                dayElem.classList.add('border', 'border-danger', 'text-danger', 'rounded-circle', 'fw-bold');
            }

            // Today -> filled secondary
            if (dateStr === todayStr) {
                dayElem.classList.add('bg-secondary', 'text-dark', 'rounded-circle', 'fw-bold');
            }
        }
    });
};
